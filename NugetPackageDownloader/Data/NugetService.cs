using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGet.Protocol;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO.Compression;

namespace NugetPackageDownloader.Data
{
    public class NugetService
    {
        public async Task<IList<string>> ListPackageVersions(string packageId)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            IEnumerable<NuGetVersion> versions = await resource.GetAllVersionsAsync(
                packageId,
                cache,
                logger,
                cancellationToken);

            var versionList = new List<string>();
            foreach (NuGetVersion version in versions)
            {
                versionList.Add(version.ToString());
            }

            return versionList;
        }

        public async Task<byte[]> DownloadPackage(string packageId, string version)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            NuGetVersion packageVersion = new NuGetVersion(version);
            using MemoryStream packageStream = new MemoryStream();

            await resource.CopyNupkgToStreamAsync(
                packageId,
                packageVersion,
                packageStream,
                cache,
                logger,
                cancellationToken);



            Console.WriteLine($"Downloaded package {packageId} {packageVersion}");

            return packageStream.ToArray();
        }

        public async Task<IList<DependencyGroup>> GetAllDependencies(string packageId, string version)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();

            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            DependencyInfoResource dependencyInfo = await repository.GetResourceAsync<DependencyInfoResource>();

            var remoteSourceDependencyInfo = await dependencyInfo.ResolvePackages(packageId, cache, logger, cancellationToken);

            RemoteSourceDependencyInfo foundPackage = remoteSourceDependencyInfo.ToList().Where(p => p.Identity.Version.OriginalVersion.Equals(version)).FirstOrDefault();

            return JsonConvert.DeserializeObject<List<DependencyGroup>>(foundPackage.DependencyGroups.ToJson());
        }

        public async Task<PackageMetadata> GetPackageMetadata(string packageId, string version)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

            PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>();

            IEnumerable<IPackageSearchMetadata> packages = await resource.GetMetadataAsync(
                packageId,
                includePrerelease: true,
                includeUnlisted: false,
                cache,
                logger,
                cancellationToken);

            IPackageSearchMetadata foundPackage = packages.Where(p => p.Identity.Version.OriginalVersion.Equals(version)).FirstOrDefault();

            return JsonConvert.DeserializeObject<PackageMetadata>(foundPackage.ToJson());
        }

        public async Task<IList<string>> GetPackageAutoComplete(string packageId)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

            AutoCompleteResource resource = await repository.GetResourceAsync<AutoCompleteResource>();

            IEnumerable<string> packages = await resource.IdStartsWith(
                packageId,
                includePrerelease: false,
                logger,
                cancellationToken
                );

            return packages.ToList();
        }

        public async Task GetAllDependencies(string frameworkName, string frameworkVersion, string packageId, string version, IList<Package> dependentPackageList, int index)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();

            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            DependencyInfoResource dependencyInfo = await repository.GetResourceAsync<DependencyInfoResource>();

            PackageIdentity identity = new PackageIdentity(packageId, new NuGetVersion(version));
            NuGetFramework framework = new NuGetFramework(frameworkName, new System.Version(frameworkVersion));

            SourcePackageDependencyInfo sourcePackageDependencyInfo = await dependencyInfo.ResolvePackage(identity, framework, cache, logger, cancellationToken);

            if (sourcePackageDependencyInfo.Dependencies != null && sourcePackageDependencyInfo.Dependencies.Count() > 0)
            {
                foreach (var dependency in sourcePackageDependencyInfo.Dependencies)
                {
                    var pack = new Package { Id = dependency.Id, VersionRange = dependency.VersionRange.MinVersion.OriginalVersion };

                    var isExist = dependentPackageList.Where(i => i.Id.Equals(pack.Id)).Any();
                    if (!isExist)
                    {
                        dependentPackageList.Add(pack);
                    }
                }
                if (index + 1 < dependentPackageList.Count)
                {
                    index += 1;
                    await GetAllDependencies(frameworkName, frameworkVersion, dependentPackageList[index].Id, dependentPackageList[index].VersionRange, dependentPackageList, index);
                }
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/49970796/download-multiple-files-contained-in-a-list-of-byte-array-in-asp-net-mvc-c-sharp
        /// </summary>
        /// <param name="zipFiles"></param>
        /// <returns></returns>
        public byte[] ZipFiles(List<ZipFile> zipFiles)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var file in zipFiles)
                    {
                        var entry = archive.CreateEntry(file.Name, CompressionLevel.Fastest);
                        using (var zipStream = entry.Open())
                        {
                            zipStream.Write(file.Content, 0, file.Content.Length);
                        }
                    }
                }

                return ms.ToArray();
            }
        }

    }
}

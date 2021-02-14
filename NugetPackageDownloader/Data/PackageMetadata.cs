using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NugetPackageDownloader.Data
{
    public class Version
    {
        [JsonProperty("Major")]
        public int Major { get; set; }

        [JsonProperty("Minor")]
        public int Minor { get; set; }

        [JsonProperty("Build")]
        public int Build { get; set; }

        [JsonProperty("Revision")]
        public int Revision { get; set; }

        [JsonProperty("MajorRevision")]
        public int MajorRevision { get; set; }

        [JsonProperty("MinorRevision")]
        public int MinorRevision { get; set; }
    }

    public class TargetFramework
    {
        [JsonProperty("Framework")]
        public string Framework { get; set; }

        [JsonProperty("Version")]
        public Version Version { get; set; }

        [JsonProperty("HasProfile")]
        public bool HasProfile { get; set; }

        [JsonProperty("Profile")]
        public string Profile { get; set; }

        [JsonProperty("DotNetFrameworkName")]
        public string DotNetFrameworkName { get; set; }

        [JsonProperty("IsPCL")]
        public bool IsPCL { get; set; }

        [JsonProperty("IsPackageBased")]
        public bool IsPackageBased { get; set; }

        [JsonProperty("AllFrameworkVersions")]
        public bool AllFrameworkVersions { get; set; }

        [JsonProperty("IsUnsupported")]
        public bool IsUnsupported { get; set; }

        [JsonProperty("IsAgnostic")]
        public bool IsAgnostic { get; set; }

        [JsonProperty("IsAny")]
        public bool IsAny { get; set; }

        [JsonProperty("IsSpecificFramework")]
        public bool IsSpecificFramework { get; set; }
    }

    public class Package
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Include")]
        public List<object> Include { get; set; }

        [JsonProperty("Exclude")]
        public List<object> Exclude { get; set; }

        [JsonProperty("VersionRange")]
        public string VersionRange { get; set; }
    }

    public class DependencyGroup
    {
        [JsonProperty("TargetFramework")]
        public TargetFramework TargetFramework { get; set; }

        [JsonProperty("Packages")]
        public List<Package> Packages { get; set; }
    }

    public class PackageMetadata
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("authors")]
        public string Authors { get; set; }

        [JsonProperty("dependencyGroups")]
        public List<DependencyGroup> DependencyGroups { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("licenseUrl")]
        public string LicenseUrl { get; set; }

        [JsonProperty("id")]
        public string SecondaryId { get; set; }

        [JsonProperty("projectUrl")]
        public string ProjectUrl { get; set; }

        [JsonProperty("published")]
        public DateTime Published { get; set; }

        [JsonProperty("requireLicenseAcceptance")]
        public bool RequireLicenseAcceptance { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("licenseExpression")]
        public string LicenseExpression { get; set; }

        [JsonProperty("listed")]
        public bool Listed { get; set; }
    }
}

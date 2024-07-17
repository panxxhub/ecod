using System.CommandLine;
using System.Xml.Serialization;
using EsiCodeGen;
using EsiInfo;
using ObjListSourceGenerator;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var command = new RootCommand();

var yamlConfigFilePathOption = new Option<FileInfo>(
	name: "--config",
	description: "Path to the YAML configuration file"
)
{
	IsRequired = true
};

var outputDirectoryOption = new Option<DirectoryInfo>(
	name: "--output",
	description: "Path to the output directory",
	getDefaultValue: () => new DirectoryInfo(Directory.GetCurrentDirectory())
)
{
	IsRequired = true
};

yamlConfigFilePathOption.AddAlias("-c");
outputDirectoryOption.AddAlias("-o");

command.AddOption(yamlConfigFilePathOption);
command.AddOption(outputDirectoryOption);

command.SetHandler((configFile, outputDirectory) =>
{
	// parse the YAML configuration file
	var yaml = File.ReadAllText(configFile.FullName);
	var deserializer = new DeserializerBuilder()
	       .WithNamingConvention(UnderscoredNamingConvention.Instance)
	       .Build();
	var config = deserializer.Deserialize<GeneratorConfig>(yaml);

	if (config.EsiFile == null)
	{
		throw new Exception("EsiFile is required");
	}

	XmlSerializer serializer = new(typeof(EtherCatInfo));
	var ethercatInfo = serializer.Deserialize(new FileStream(config.EsiFile, FileMode.Open)) as EtherCatInfo ?? throw new Exception("Failed to parse ESI file");
	var device = ethercatInfo.Descriptions.Devices.FirstOrDefault(x => ParseExtensions.ParseEsiHexCode(x.Type.ProductCode) == config.DeviceCode) ?? throw new Exception($"Failed to Find {config.DeviceCode}");

	var profile = device.Profile[config.ProfileIndex] ?? throw new Exception("Profile not found");
	// profile.Dictionary.DataTypes
	// profile.Dictionary

	// var obj_list = profile.Dictionary.Objects.ToList();
	var tt = new ObjsTemplate(profile);

	var t_str = tt.TransformText();

	var outputFilePath = Path.Combine(outputDirectory.FullName, "obj_list_gen.c");
	File.WriteAllText(outputFilePath, t_str);






}, yamlConfigFilePathOption, outputDirectoryOption);

await command.InvokeAsync(args);
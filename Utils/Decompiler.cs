using DevTools.Viewers;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace DevTools.Utils;

internal class Decompiler : ILoadable
{
	private static Dictionary<string, CSharpDecompiler> decompilers;
	private static Dictionary<string, PEFile> pes;
	private static DecompilerSettings Settings;

	internal static void AddType(ModType modItem)
	{
		var decompiler = DecompileMod(modItem.Mod);
		var name = new FullTypeName(modItem.GetType().FullName);
		var code = decompiler.DecompileTypeAsString(name);
		CodeViewer.Add(modItem.Name + ".cs", name.ReflectionName + ".cs", code);
	}

	private static CSharpDecompiler DecompileMod(Mod mod)
	{
		if (decompilers.ContainsKey(mod.Name)) return decompilers[mod.Name];

		Settings ??= new DecompilerSettings()
		{
			ThrowOnAssemblyResolveErrors = false,
			AlwaysQualifyMemberReferences = true,
			FileScopedNamespaces = true
		};
		var pe = PEForMod(mod, Settings);

		var resolver = new ModResolver(mod, Settings);

		var decompiler = new CSharpDecompiler(pe, resolver, Settings);
		decompilers.Add(mod.Name, decompiler);
		return decompiler;
	}

	internal static PEFile PEForMod(Mod mod, DecompilerSettings settings = null)
	{
		if (pes.ContainsKey(mod.Name))
			return pes[mod.Name];
		var bytes = mod.GetFileBytes(mod.Name + ".dll");
		using var stream = new MemoryStream(bytes);
		settings ??= new();
		var pe = new PEFile(mod.Name + ".dll", stream, streamOptions: PEStreamOptions.PrefetchEntireImage,
			metadataOptions: settings.ApplyWindowsRuntimeProjections ? MetadataReaderOptions.ApplyWindowsRuntimeProjections : MetadataReaderOptions.None);
		pes.Add(mod.Name, pe);
		return pe;
	}

	public void Load(Mod mod)
	{
		decompilers = new();
		pes = new();
		ModResolver.cachePEs = new();
		pes.Add("tModLoader", new PEFile(typeof(Mod).Assembly.Location));
		pes.Add("ModLoader", new PEFile(typeof(Mod).Assembly.Location));
	}

	public void Unload()
	{
		decompilers.Clear();
		foreach (var item in pes)
		{
			item.Value.Dispose();
		}
		pes.Clear();
		Settings = null;
		foreach (var item in ModResolver.cachePEs)
		{
			item.Value.Dispose();
		}
		ModResolver.cachePEs.Clear();
	}

}


public class ModResolver : IAssemblyResolver
{
	readonly IAssemblyResolver resolver;
	private Mod mod;
	private string dir;

	internal static Dictionary<string, PEFile> cachePEs;

	public ModResolver(Mod mod, DecompilerSettings settings)
	{
		this.mod = mod;
		var PE = Decompiler.PEForMod(mod, settings);
		var file = Assembly.GetEntryAssembly().Location;
		resolver = new UniversalAssemblyResolver(file, settings.ThrowOnAssemblyResolveErrors,
			PE.DetectTargetFrameworkId(), PE.DetectRuntimePack(),
			settings.LoadInMemory ? PEStreamOptions.PrefetchMetadata : PEStreamOptions.Default,
			settings.ApplyWindowsRuntimeProjections ? MetadataReaderOptions.ApplyWindowsRuntimeProjections : MetadataReaderOptions.None);
		var tmod = Assembly.GetEntryAssembly().Location;
		dir = Path.Combine(Path.GetDirectoryName(tmod), "Libraries");
	}

	public PEFile? Resolve(IAssemblyReference reference)
	{
		if(cachePEs.ContainsKey(reference.Name))
		{
			return cachePEs[reference.Name];
		}
		var ret = resolveinternal(reference);
		if (ret != null) cachePEs.Add(reference.Name, ret);
		return ret;
	}
	
	private PEFile resolveinternal(IAssemblyReference reference)
	{
		var libname = Path.Combine(dir, reference.Name);
		if (Directory.Exists(libname))
		{
			var file = Directory.EnumerateFiles(libname, reference.Name + ".dll", SearchOption.AllDirectories).FirstOrDefault();
			if (!string.IsNullOrWhiteSpace(file))
			{
				return new PEFile(file);
			}
		}
		if (ModLoader.TryGetMod(reference.Name, out Mod mod))
		{
			return Decompiler.PEForMod(mod);
		}
		return resolver.Resolve(reference);
	}

	public PEFile? ResolveModule(PEFile mainModule, string moduleName)
	{
		return resolver.ResolveModule(mainModule, moduleName);
	}

	public Task<PEFile?> ResolveAsync(IAssemblyReference reference)
	{
		var ret = Resolve(reference);
		return Task.FromResult(ret);
	}

	public Task<PEFile?> ResolveModuleAsync(PEFile mainModule, string moduleName)
	{
		var ret = ResolveModule(mainModule, moduleName);
		return Task.FromResult(ret);
	}
}
using System;
using System.Linq;
using System.Reflection;

class P {
    static void Main() {
        var types = new[] {
            "Avalonia.Input.Platform.IClipboard",
            "Avalonia.Platform.ICursorFactory",
            "Avalonia.Platform.Storage.IStorageFolder",
            "Avalonia.Platform.Storage.IStorageProvider",
            "Avalonia.Platform.IWindowingPlatform",
            "Avalonia.Rendering.IRenderTimer",
            "Avalonia.Platform.ITopLevelImpl",
            "Avalonia.Platform.Surfaces.IPlatformRenderSurface",
            "Avalonia.Skia.ISkiaGpu",
            "Avalonia.Skia.ISkiaGpuRenderTarget",
            "Avalonia.Skia.ISkiaGpuRenderSession",
            "Avalonia.Skia.ISkiaSurface",
            "Avalonia.Input.IKeyboardNavigationHandler",
            "Avalonia.Platform.IPlatformGraphics",
            "Avalonia.Platform.IPlatformGraphicsContext",
            "Avalonia.Platform.IRenderTarget",
            "Avalonia.Platform.IAsyncDataTransfer",
            "Avalonia.Platform.Storage.StorageFilePickerResult",
        };
        var baseDir = @"C:\Users\22165\.nuget\packages\avalonia\12.0.0\lib\net10.0";
        var skiaDir = @"C:\Users\22165\.nuget\packages\avalonia.skia\12.0.0\lib\net10.0";
        foreach (var d in new[] { baseDir, skiaDir }) {
            foreach (var f in System.IO.Directory.GetFiles(d, "*.dll")) {
                try { Assembly.LoadFrom(f); } catch {}
            }
        }
        foreach (var tn in types) {
            var t = Type.GetType(tn + ", Avalonia.Base") ?? Type.GetType(tn + ", Avalonia.Skia") ?? AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => { try { return a.GetTypes(); } catch { return Type.EmptyTypes; } }).FirstOrDefault(x => x.FullName == tn);
            if (t == null) { Console.WriteLine("NOT FOUND: " + tn); continue; }
            Console.WriteLine("\n=== " + t.FullName + " (" + (t.IsPublic ? "public" : t.IsNotPublic ? "internal" : "?") + ") in " + t.Assembly.GetName().Name + " ===");
            if (t.IsInterface) DumpType(t);
        }
        // InternalsVisibleTo
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name?.StartsWith("Avalonia") == true)) {
            foreach (var attr in asm.GetCustomAttributesData()) {
                if (attr.AttributeType.FullName == "System.Runtime.CompilerServices.InternalsVisibleToAttribute")
                    Console.WriteLine("IVT: " + asm.GetName().Name + " -> " + attr.ConstructorArguments[0].Value);
            }
        }
        // Search IPlatformRenderSurface implementors and related
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name?.StartsWith("Avalonia") == true)) {
            Type[] ts;
            try { ts = asm.GetTypes(); } catch { continue; }
            foreach (var t in ts.Where(x => x.IsPublic && (x.Name.Contains("RenderSurface") || x.Name.Contains("SkiaGpu") || x.Name.Contains("ExternalObject") || x.Name.Contains("Vulkan") || x.Name.Contains("PlatformGraphics")))) {
                if (t.IsInterface || t.IsAbstract) {
                    Console.WriteLine("\n=== RELATED PUBLIC: " + t.FullName + " ===");
                    DumpType(t);
                }
            }
        }
    }
    static void DumpType(Type t) {
        foreach (var m in t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).OrderBy(x => x.Name)) {
            if (m is MethodInfo mi && mi.IsSpecialName) continue;
            if (m is PropertyInfo pi) Console.WriteLine("  " + (pi.GetMethod?.IsPublic == true ? "public" : pi.GetMethod?.IsAssembly == true ? "internal" : "private") + " " + pi.PropertyType.Format() + " " + pi.Name + " { get; set; }");
            else if (m is MethodInfo meth) {
                var ps = string.Join(", ", meth.GetParameters().Select(p => p.ParameterType.Format() + " " + p.Name));
                Console.WriteLine("  " + (meth.IsPublic ? "public" : meth.IsAssembly ? "internal" : "private") + " " + meth.ReturnType.Format() + " " + meth.Name + "(" + ps + ")");
            }
            else if (m is EventInfo ev) Console.WriteLine("  event " + ev.EventHandlerType?.Format() + " " + ev.Name);
            else if (m is FieldInfo fi) Console.WriteLine("  field " + fi.FieldType.Format() + " " + fi.Name);
        }
    }
}
static class Ext { public static string Format(this Type t) => t.FullName?.Replace("+", ".") ?? t.Name; }

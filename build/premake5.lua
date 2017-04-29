workspace "UnityPluginWrapper"
	configurations {"Debug", "Release"}
	platforms {"Win32", "Win64"}
	language "C#"
	
	


project "UnityPluginWrapperDLLs" 
	kind "SharedLib"
	dotnetframework "3.5"
	targetdir "bin/%{cfg.buildcfg}/%{cfg.platform}"
	targetname "NSLoaderWrapper"
	clr "Unsafe"

	libdirs {"C:/Program Files/Unity 5-4-1f1/Editor/Data/Managed/"}

	links {"System", "UnityEditor", "UnityEngine", "System.ServiceProcess"}

	files {
		"../src/*.cs"
	}

	

	filter "platforms:Win32" 
		system "Windows"
		architecture "x86"
	filter "platforms:Win64"
		system "Windows"
		architecture "x86_64"

	filter "configurations:Debug"
		defines {"DEBUG", "TRACE"}
	filter "configurations:Release"
		optimize "On" 



project "UnityPluginWrapperExe"
	kind "ConsoleApp"
	
	targetdir "bin/%{cfg.buildcfg}/%{cfg.platform}"
	clr "Unsafe"

	libdirs {"C:/Program Files/Unity 5-4-1f1/Editor/Data/Managed/"}

	links {"System", "UnityEditor", "UnityEngine", "System.ServiceProcess"}

	files {
		"../src/*.cs"
	}

	-- note that we are using 4.5 for native debugging. But this also means it will compile
	-- newer code that isn't Unity compatible. 
	dotnetframework "4.5"

	filter "platforms:Win32" 
		system "Windows"
		architecture "x86"
	filter "platforms:Win64"
		system "Windows"
		architecture "x86_64"

	filter "configurations:Debug"
		defines {"DEBUG", "TRACE"}
	filter "configurations:Release"
		optimize "On" 
	

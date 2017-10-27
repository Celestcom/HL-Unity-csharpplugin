# D:\Users\Projects\Unity Csharp Plugin\build\bin\Release\Win32
# to
# D:\Users\Projects\Unity-SDK\Assets\NullSpace SDK\Plugins\x86

set dllx32="D:\Users\Projects\Unity Csharp Plugin\build\bin\Release\Win32\HardlightUnity.dll"
set xmlx32="D:\Users\Projects\Unity Csharp Plugin\build\bin\Release\Win32\HardlightUnity.XML"

set destinationx86="D:\Users\Projects\Unity-SDK\Assets\NullSpace SDK\Plugins\x86\"

set dllx64="D:\Users\Projects\Unity Csharp Plugin\build\bin\Release\Win64\HardlightUnity.dll"
set xmlx64="D:\Users\Projects\Unity Csharp Plugin\build\bin\Release\Win64\HardlightUnity.XML"

set destinationx64="D:\Users\Projects\Unity-SDK\Assets\NullSpace SDK\Plugins\x86_64\"

xcopy %dllx32% %destinationx86% /y
xcopy %xmlx32% %destinationx86% /y

xcopy %dllx64% %destinationx64% /y
xcopy %xmlx64% %destinationx64% /y
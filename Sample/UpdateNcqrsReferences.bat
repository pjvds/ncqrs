#copy "..\Framework\bin\log4net.dll" "bin"
#copy "..\Framework\bin\MongoDB.Driver.dll" "bin"
#copy "..\Framework\bin\MongoDB.Emitter.dll" "bin"
#copy "..\Framework\bin\Newtonsoft.Json.Net20.dll" "bin"
#copy "..\Framework\bin\StructureMap.dll" "bin"

copy "..\Framework\src\Ncqrs\bin\Debug\Ncqrs.dll" "bin\Ncqrs"
copy "..\Framework\src\Ncqrs\bin\Debug\Ncqrs.pdb" "bin\Ncqrs"
copy "..\Framework\src\Ncqrs\bin\Debug\Ncqrs.xml" "bin\Ncqrs"

copy "..\Framework\src\Ncqrs.Config.StructureMap\bin\Debug\Ncqrs.Config.StructureMap.dll" "bin\Ncqrs"
copy "..\Framework\src\Ncqrs.Config.StructureMap\bin\Debug\Ncqrs.Config.StructureMap.pdb" "bin\Ncqrs"
copy "..\Framework\src\Ncqrs.Config.StructureMap\bin\Debug\Ncqrs.Config.StructureMap.xml" "bin\Ncqrs"

copy "..\Framework\src\Ncqrs.Eventing.Storage.MongoDB\bin\Debug\Ncqrs.Eventing.Storage.MongoDB.dll" "bin\Ncqrs"
copy "..\Framework\src\Ncqrs.Eventing.Storage.MongoDB\bin\Debug\Ncqrs.Eventing.Storage.MongoDB.pdb" "bin\Ncqrs"
copy "..\Framework\src\Ncqrs.Eventing.Storage.MongoDB\bin\Debug\Ncqrs.Eventing.Storage.MongoDB.xml" "bin\Ncqrs"
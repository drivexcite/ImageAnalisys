az login;
az group create -l westus -n CognitiveServicesGroup;
az cognitiveservices account create -g CognitiveServicesGroup -l westus --kind ComputerVision -n ImageAnalysisService --sku S1 --yes;
$cognitiveServicesKey = az cognitiveservices account keys list -n ImageAnalysisService -g CognitiveServicesGroup --query key1;
$cognitiveServicesEndpoint = az cognitiveservices account show -n ImageAnalysisService -g CognitiveServicesGroup --query endpoint;
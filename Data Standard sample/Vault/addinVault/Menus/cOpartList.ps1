#[System.Reflection.Assembly]::LoadFrom("C:\Program Files (x86)\Autodesk\Autodesk Vault 2014 SDK\bin\Autodesk.Connectivity.WebServices.dll")
#$cred = New-Object Autodesk.Connectivity.WebServicesTools.UserPasswordCredentials("localhost", "Vault", "Administrator", "")
#$vault = New-Object Autodesk.Connectivity.WebServicesTools.WebServiceManager($cred)

$id = $vaultContext.CurrentSelectionSet[0].Id
#$id = 67
$file = $vault.DocumentService.GetLatestFileByMasterId($id)

#getting all the file references
$assocs = $vault.DocumentService.GetFileAssociationsByIds(@($file.Id),[Autodesk.Connectivity.WebServices.FileAssociationTypeEnum]::None,$false,[Autodesk.Connectivity.WebServices.FileAssociationTypeEnum]::Dependency,$true,$fale,$false)

#region idetifying childredn only
$parentIDs = @()
$assocs[0].FileAssocs | ForEach-Object { $parentIDs += $_.ParFile.MasterId }

$pureChildren = @{}
foreach($assoc in $assocs[0].FileAssocs)
{
	if($parentIDs -notcontains $assoc.Cldfile.MasterID -and $pureChildren.Keys -notcontains $assoc.Cldfile.Id) 
	{
		$pureChildren[$assoc.Cldfile.Id] += $assoc.Cldfile 
	}
}
#endregion

#region collecting additional file propertiy ids
$propDefs = $vault.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE")
$propNames = @("Description","Part Number","Material")
$propDefIds = @{}
foreach($name in $propNames) {
	$propDef = $propDefs | Where-Object { $_.DispName -eq $name }
	$propDefIds[$propDef.Id] = $propDef.DispName
}
#endregion

#region collecting additional property values
$props = $vault.PropertyService.GetProperties("FILE",$pureChildren.Keys,$propDefIds.Keys)
$propArray = @{}
$props | ForEach-Object { 
if($propArray.Keys -notcontains $_.EntityId) { $propArray.Add($_.EntityId,@{}) }
	$propArray[$_.EntityId][$propDefIds[$_.PropDefId]] = $_.Val 
}
#endregion

#region preparing the DataTable with according columns
$dt = New-Object System.Data.DataTable("DataSet1")
$dt.Columns.Add("FileName")
$dt.Columns.Add("Revision")
$dt.Columns.Add("Category")
$dt.Columns.Add("Description")
$dt.Columns.Add("PartNumber")
$dt.Columns.Add("Material")
$dt.Columns.Add("State")
$dt.Columns.Add("ItemNum")
$dt.Columns.Add("ItemRev")
#endregion
cls
#region filling the DataTable with values
foreach ($child in $pureChildren.Values) {
	$dr = $dt.NewRow()
	$dr.FileName = $child.Name
	$child.Name
	$child.FileRev.Label
	$dr.Revision = $child.FileRev.Label
	$dr.Category = $child.Cat.CatName
	$dr.Description = $propArray[$child.Id]["Description"]
	$dr.PartNumber = $propArray[$child.Id]["Part Number"]
	$dr.Material = $propArray[$child.Id]["Material"]
	$dr.State = $child.FileLfCyc.LfCycStateName
	$items = $vault.ItemService.GetItemsByFileId($child.Id)
	if($items.Count -gt 0)
	{
		$dr.ItemNum = $items[0].ItemNum
		$dr.ItemRev = $items[0].RevNum
	}
	$dt.Rows.Add($dr)
}

#endregion

#region collecting additional information for the report
$folderId = $vaultContext.NavSelectionSet[0].Id
$folder = $vault.DocumentService.GetFolderById($FolderId)
$params = @{}
$params["Project"] = $folder.Name
$params["Assembly"] = $file.Name
#endregion

[System.Reflection.Assembly]::LoadFrom("C:\ProgramData\Autodesk\Vault 2014\Extensions\DataStandard\Vault\addinVault\Menus\cOreporter.dll")
$report = New-Object cOreporter.cOreporter("C:\ProgramData\Autodesk\Vault 2014\Extensions\DataStandard\Vault\addinVault\Menus\partList.rdlc",$dt,$params)




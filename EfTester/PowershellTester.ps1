$configpath = "C:\Users\kenny\Documents\Developer\KenProjects\EfTester\CcsEnityFramework\App.Config"
[appdomain]::CurrentDomain.SetData("APP_CONFIG_FILE", $configpath)
[Configuration.ConfigurationManager].GetField("s_initState", "NonPublic, Static").SetValue($null, 0)
[Configuration.ConfigurationManager].GetField("s_configSystem", "NonPublic, Static").SetValue($null, $null)
([Configuration.ConfigurationManager].Assembly.GetTypes() | where {$_.FullName -eq "System.Configuration.ClientConfigPaths"})[0].GetField("s_current", "NonPublic, Static").SetValue($null, $null)

Import-Module "C:\Users\kenny\Documents\Developer\KenProjects\EfTester\CcsEnityFramework\bin\Debug\CcsEnityFramework.dll"

$snapshotsToProcess = @(
1010
)

$start = Get-Date

$snapshotsToProcess | % {
    [CcsEnityFramework.CcsEfDll]::processSnapshot($_)
}

$end = Get-Date
Write-Host ($end - $start)




#[appdomain]::CurrentDomain.GetData("APP_CONFIG_FILE")


#begin document
Import-Module "C:\Users\kenny\Documents\Developer\Afp\Dans\AFPUtils(1.0.18.338.0)\AFPUtils.dll"
$parser = New-Object AFPUtils.Parser
$Concatenator = New-Object AFPUtils.Concatenator



#process

gci "C:\Users\kenny\Documents\Developer\Afp\Dans\files\" | % {
Write-Host "*************New Doc*****************"
    $ccsDoc = New-Object PSObject
    Add-Member -InputObject $ccsDoc -MemberType NoteProperty -Name ProcessingExceptionCode -Value "0"
    
    $files = gci $_.FullName


    ########This is where the Ccs Operation begins#################
#************NEED TO CHANGE ROOT PATH
    $rootPath = $_.FullName
    $afpDoc = New-Object "System.Collections.Generic.List``1[AFPUtils.AfpDocument]"

    $count = 1
    #loop through files
    $files | % {
        #read the bytes and parse the afp segments
##***********NEED TO CALL CCS READ*******************
        $b = [System.IO.File]::ReadAllBytes($_.FullName)
        $p = $parser.Parse($b)

        #check if the afp group is valid
        $isResource = $p.IsResourceFile
        $beginResourceCount = ($p.Segments | ? { $_.SegmentType -eq "BEGIN_RESOURCE_GROUP" } | Group-Object -Property SegmentType).Count
        $endResourceCount = ($p.Segments | ? { $_.SegmentType -eq "END_RESOURCE_GROUP" } | Group-Object -Property SegmentType).Count
        $beginDocumentCount = ($p.Segments | ? { $_.SegmentType -eq "BEGIN_DOCUMENT" } |  Group-Object -Property SegmentType).Count
        $endDocumentCount = ($p.Segments | ? { $_.SegmentType -eq "END_DOCUMENT" } | Group-Object -Property SegmentType).Count

        if($count -eq 1 -and $beginResourceCount -eq 1 -and $beginDocumentCount -eq 0 -and $endDocumentCount -eq 0)
        {
            "I'm a valid resource group"
            $afpDoc.Add($p)
        }
        elseif($beginResourceCount -eq 0 -and $beginDocumentCount -eq 1 -and $endDocumentCount -eq 1)
        {
            "I'm a valid document"
            $afpDoc.Add($p)
        }
        else 
        {
#************Change to $doc.processing_exception_code**********
            
            #exception codes
            if($beginResourceCount -eq 1 -and $count -gt 1)
            {
                $ccsDoc.ProcessingExceptionCode = "1000"
                write-host ("Afp resource not found as page 1")
            }
            elseif($beginResourceCount -gt 1)
            {
                Write-Host ("More than 1 begin resource found")
                $ccsDoc.ProcessingExceptionCode = "1001"
            }
            elseif($beginDocumentCount -gt 1)
            {
                Write-Host ("More than 1 begin document found")
                $ccsDoc.ProcessingExceptionCode = "1002"
            } 
            elseif($beginResourceCount -gt 0 -and $beginDocumentCount -gt 0)
            {
                $ccsDoc.ProcessingExceptionCode = "1003"
                Write-Host ("Found combination of begin resource and begin document")
            }
            else 
            {
                $ccsDoc.ProcessingExceptionCode = "1004"
                Write-Host "Other AFP exception found"
            }
        }

        $count ++
    }
    
#************Change to $doc.processing_exception_code**********
    if($ccsDoc.ProcessingExceptionCode -eq "0")
    {
        $conFile = $Concatenator.Concatenate($afpDoc)
##***********NEED TO CALL CCS WRITE*******************
        $out = Join-Path $rootPath -ChildPath "Concatenated.afp"
        [System.IO.File]::WriteAllBytes($out, $conFile)
    }

}




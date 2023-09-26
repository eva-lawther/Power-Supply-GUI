
$selectscript = $args[0]
Switch ($selectscript)
{

    default {
		Write-Output "PowerShell: script not found:"
		Write-Output $selectscript
	}
}

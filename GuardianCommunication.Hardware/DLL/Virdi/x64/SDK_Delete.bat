cd /d %~dp0
if /i "%PROCESSOR_IDENTIFIER:~0,3%"=="X86" (
	echo "system is x86 and DLLs cannot be registered"
	) else (
		regsvr32 /s /c %windir%\system32\UCSAPICOM.dll -u
		del %windir%\system32\FPLib.dll
		del %windir%\system32\Interop.UCBioBSPCOMLib.dll
		del %windir%\system32\Interop.UCSAPICOMLib.dll
		del %windir%\system32\NSearchMC.dll
		del %windir%\system32\UCBioBSP.dll
		del %windir%\system32\UCBioBSPCOM.dll
		del %windir%\system32\UCSAPI40.dll
		del %windir%\system32\UCSAPICOM.dll
		del %windir%\system32\VHMLib.dll
		del %windir%\system32\VirdiFP.dll
		del %windir%\system32\WSEngine.dll
	)

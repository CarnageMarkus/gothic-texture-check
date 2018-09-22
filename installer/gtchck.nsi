SetCompressor /solid lzma 

!include utils.nsi

!include ..\.ver

XPStyle on

RequestExecutionLevel user

PageEx license
  LicenseData "..\EULA"
  LicenseForceSelection checkbox
PageExEnd

Page license
Page instfiles

LoadLanguageFile "${NSISDIR}\Contrib\Language files\English.nlf"

LicenseLangString myLicenseData ${LANG_ENGLISH} "..\LICENSE"
LicenseData $(myLicenseData)

LangString Name ${LANG_ENGLISH} "English"

Name "Gothic Texture Check"
OutFile "gothicTextureCheck-${GTCHCK_VERSION}.exe"
InstallDir $APPDATA\GTCHCK
InstallDirRegKey HKCU "Software\GTCHCK" "Install_Dir"

!define APP_NAME "Gothic Texture Check" 
!define APP_COPY "Copyright © 2018 Marek 'Carnage' Karas" 
!define APP_COMP "Marek 'Carnage' Karas"                 

VIProductVersion ${GTCHCK_VERSION_FULL}
VIAddVersionKey "CompanyName"      "${APP_COMP}"
VIAddVersionKey "FileVersion"      "${VER_TEXT}"
VIAddVersionKey "LegalCopyright"   "${APP_COPY}"
VIAddVersionKey "FileDescription"  "${APP_NAME}"

Function .onInit
  Call IsSilent
  Pop $0
  StrCmp $0 1 0 +2
	goto end
	
	Push ""
	Push ${LANG_POLISH}
	Push Polski
	Push ${LANG_ENGLISH}
	Push English
	Push A
	LangDLL::LangDialog "Installer Language" "Please select the language of the installer"

	Pop $LANGUAGE
	StrCmp $LANGUAGE "cancel" 0 +2
		Abort
		
	end:
FunctionEnd

Section "" 
  SetOutPath "$INSTDIR"
  File "..\README.md"
  File "..\ThirdPartyNotices.md"
  File "..\CHANGELOG.md"
  File "..\LICENSE"
  
  SetOutPath "$INSTDIR\bin"
  File "..\src\bin\release\netcoreapp2.1\win-x86\publish\*.exe"
  File "..\src\bin\release\netcoreapp2.1\win-x86\publish\*.dll"
  
  WriteRegStr HKCU SOFTWARE\GTCHCK "Install_Dir" "$INSTDIR"
   
  Push "$INSTDIR\bin"
  Call AddToPath

  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GTCHCK"
  
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GTCHCK" "DisplayName" "Gothic Texture Check"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GTCHCK" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GTCHCK" "NoModify" 1
  WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GTCHCK" "NoRepair" 1
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GTCHCK" "Readme" "https://github.com/CarnageMarkus/gothic-texture-check"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GTCHCK" "Publisher" "Marek 'Carnage' Karas"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GTCHCK" "DisplayVersion" "${GTCHCK_VERSION}"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GTCHCK" "Comments" "Simple command line tool which checks folders for .tga or .TEX files and report if there are any missing animated or varied textures."
  WriteUninstaller "uninstall.exe"
	
SectionEnd

Section "Uninstall"
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\GTCHCK"
  DeleteRegKey HKCU "Software\GTCHCK"

  Delete $INSTDIR\uninstall.exe
  
  Push "$INSTDIR\bin"
  Call un.RemoveFromPath

  Delete $INSTDIR\bin\*
  Delete $INSTDIR\*
  
  RMDir /R "$INSTDIR\bin"

SectionEnd

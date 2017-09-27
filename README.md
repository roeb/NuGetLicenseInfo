# NuGetLicenseInfo
Extract the license infos from package.config files and saved in a markdown file


## Sample
```NugetLicenseInfo.exe --packages=C:\MyProject1\packages.config,C:\MyProject2\packages.config --output=C:\Temp\licenseInfos.md```

### Parameters:

Source path to the package.config files. You can choose as many as you like. They must be separated with a comma.

  ```--packages=[package.config PATH],[package.config PATH], ...```
  
Output folder and filename. The folder does not have to exist.

  ```--output=[PATH_TO_MD_FILE.md]```

### Output Sample:
[NugetLicenses.md](https://github.com/roeb/NuGetLicenseInfo/blob/master/NugetLicenses.md)

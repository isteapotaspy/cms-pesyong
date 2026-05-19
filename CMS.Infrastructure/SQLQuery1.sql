UPDATE PackageSizes
SET IsAvailable = 1
WHERE IsAvailable = 0;

UPDATE PackageSelectionRules
SET IsActive = 1
WHERE IsActive = 0;

UPDATE PackageSelectionOptions
SET IsActive = 1
WHERE IsActive = 0;
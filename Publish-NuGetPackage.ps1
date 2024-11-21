# Publish-NuGetPackage.ps1

# Variables
$solutionPath = ".\JsonFlatten.sln"
$projectPath = ".\JsonFlatten\JsonFlatten.csproj"
$testProjectPath = ".\JsonFlatten.Tests\JsonFlatten.Tests.csproj"
$nugetSource = "https://api.nuget.org/v3/index.json"
$nugetApiKey = $env:NUGET_API_KEY

# Ensure the NuGet API key is set
if ( [string]::IsNullOrEmpty($nugetApiKey))
{
    Write-Error "NuGet API key is not set in the environment variable 'NUGET_API_KEY'."
    exit 1
}

# Step 1: Clean the solution
Write-Host "Cleaning the solution..."
dotnet clean $solutionPath -c Release

# Step 2: Build the solution in Release mode
Write-Host "Building the solution in Release mode..."
dotnet build $solutionPath -c Release

# Step 3: Run the tests
Write-Host "Running the tests..."
dotnet test $testProjectPath -c Release

if ($LASTEXITCODE -ne 0)
{
    Write-Error "Tests failed. Aborting."
    exit 1
}

# Step 4: Pack the project to create a NuGet package
Write-Host "Packing the project to create a NuGet package..."
dotnet pack $projectPath -c Release -o ./nupkg

# Step 5: Find the generated .nupkg file
$nupkgFiles = Get-ChildItem -Path "./nupkg" -Filter "*.nupkg" | Sort-Object LastWriteTime -Descending
if ($nupkgFiles.Count -eq 0)
{
    Write-Error "No .nupkg file found in the ./nupkg directory."
    exit 1
}
else
{
    $nupkgFilePath = $nupkgFiles[0].FullName
    Write-Host "Found package: $nupkgFilePath"
}

# Step 6: Push the package to NuGet.org
Write-Host "Pushing the package to NuGet.org..."
dotnet nuget push $nupkgFilePath -k $nugetApiKey -s $nugetSource

Write-Host "NuGet package published successfully!"

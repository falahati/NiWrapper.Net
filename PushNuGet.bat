pushd "./Solution Items"
for %%F in (../Packages/NuGet/*.*) do (
   NuGet push "../Packages/NuGet/%%~F"
)
popd
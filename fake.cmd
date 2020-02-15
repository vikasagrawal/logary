SET TOOL_PATH=.fake


IF NOT EXIST "%TOOL_PATH%\fake.exe" (
  dotnet tool install fake-cli --tool-path ./%TOOL_PATH%
)

dotnet tool install paket --tool-path .paket --version 5.242.1

"%TOOL_PATH%/fake.exe" %*

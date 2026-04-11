Set-Location $PSScriptRoot

$diretorioRelatorio = "src/Tests/Report"
$projetoTestes = "src/Tests/Tests.csproj"
$arquivoConfiguracaoCobertura = "src/Tests/coverlet.runsettings"

Write-Host "Limpando relatorios antigos..." -ForegroundColor Cyan
if (Test-Path $diretorioRelatorio) {
    Remove-Item $diretorioRelatorio -Recurse -Force
}
New-Item -ItemType Directory -Path $diretorioRelatorio -Force | Out-Null

Write-Host "Executando testes com cobertura..." -ForegroundColor Cyan
dotnet test $projetoTestes --collect:"XPlat Code Coverage" --results-directory $diretorioRelatorio --settings $arquivoConfiguracaoCobertura

if ($LASTEXITCODE -eq 0) {
    Write-Host "Testes executados com sucesso!" -ForegroundColor Green

    if (-not (Get-Command "reportgenerator" -ErrorAction SilentlyContinue)) {
        Write-Host "Instalando ReportGenerator..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-reportgenerator-globaltool
    }

    $arquivoCobertura = Get-ChildItem -Path $diretorioRelatorio -Filter "coverage.cobertura.xml" -Recurse | Select-Object -First 1
    if ($arquivoCobertura) {
        Write-Host "Gerando relatorio HTML..." -ForegroundColor Cyan
        reportgenerator "-reports:$($arquivoCobertura.FullName)" "-targetdir:src/Tests/Report/html" "-reporttypes:Html"

        if ($LASTEXITCODE -eq 0) {
            Write-Host "Relatorio HTML gerado em: src/Tests/Report/html/index.html" -ForegroundColor Green
        }
    }
} else {
    Write-Host "Falha na execucao dos testes" -ForegroundColor Red
    exit 1
}
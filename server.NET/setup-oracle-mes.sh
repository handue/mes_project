#!/bin/bash

# 프로젝트 이름 설정 - MES 프로젝트로 변경
# Project Name
PROJECT_NAME="OracleMES"

# 솔루션 생성
# Create solution
echo "Creating Oracle MES solution..."
dotnet new sln -n $PROJECT_NAME

# 프로젝트 생성
# Create projects
echo "Creating MES projects..."
dotnet new webapi -n "$PROJECT_NAME.API"
dotnet new classlib -n "$PROJECT_NAME.Core"
dotnet new classlib -n "$PROJECT_NAME.Infrastructure"
dotnet new xunit -n "$PROJECT_NAME.Tests"

# 솔루션에 프로젝트 추가
# Add projects to solution
echo "Adding projects to solution..."
dotnet sln add "$PROJECT_NAME.API/$PROJECT_NAME.API.csproj"
dotnet sln add "$PROJECT_NAME.Core/$PROJECT_NAME.Core.csproj"
dotnet sln add "$PROJECT_NAME.Infrastructure/$PROJECT_NAME.Infrastructure.csproj"
dotnet sln add "$PROJECT_NAME.Tests/$PROJECT_NAME.Tests.csproj"

# 프로젝트 간 참조 추가
# Add project references
echo "Adding project references..."
dotnet add "$PROJECT_NAME.API/$PROJECT_NAME.API.csproj" reference "$PROJECT_NAME.Core/$PROJECT_NAME.Core.csproj"
dotnet add "$PROJECT_NAME.API/$PROJECT_NAME.API.csproj" reference "$PROJECT_NAME.Infrastructure/$PROJECT_NAME.Infrastructure.csproj"
dotnet add "$PROJECT_NAME.Infrastructure/$PROJECT_NAME.Infrastructure.csproj" reference "$PROJECT_NAME.Core/$PROJECT_NAME.Core.csproj"
dotnet add "$PROJECT_NAME.Tests/$PROJECT_NAME.Tests.csproj" reference "$PROJECT_NAME.API/$PROJECT_NAME.API.csproj"
dotnet add "$PROJECT_NAME.Tests/$PROJECT_NAME.Tests.csproj" reference "$PROJECT_NAME.Core/$PROJECT_NAME.Core.csproj"
dotnet add "$PROJECT_NAME.Tests/$PROJECT_NAME.Tests.csproj" reference "$PROJECT_NAME.Infrastructure/$PROJECT_NAME.Infrastructure.csproj"

# Oracle 및 MES 관련 NuGet 패키지 설치
# Install Oracle and MES related NuGet packages
echo "Installing Oracle MES packages..."

# API 프로젝트 패키지
cd "$PROJECT_NAME.API"
# Oracle 데이터베이스 ORM (SQL Server → Oracle로 변경)
# Oracle database ORM
dotnet add package Oracle.EntityFrameworkCore --version 8.0.2
# Oracle 관리 데이터 액세스
# Oracle Managed Data Access
dotnet add package Oracle.ManagedDataAccess.Core --version 23.5.0
# EF Core 마이그레이션 도구
# EF Core migration tools
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.5
# EF Core 도구
# EF Core tools
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.5
# JWT 인증
# JWT authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.5
# 객체 매핑 도구
# Object mapping tool
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
# 입력 데이터 유효성 검사
# Input data validation
dotnet add package FluentValidation.AspNetCore --version 11.3.0
# API 문서화 도구
# API documentation tool
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
# SignalR (실시간 MES 데이터용)
# SignalR for real-time MES data
dotnet add package Microsoft.AspNetCore.SignalR --version 1.1.0
# CORS 지원
# CORS support
dotnet add package Microsoft.AspNetCore.Cors --version 2.2.0
cd ..

# Infrastructure 프로젝트 패키지
cd "$PROJECT_NAME.Infrastructure"
# Oracle EF Core
dotnet add package Oracle.EntityFrameworkCore --version 8.0.2
dotnet add package Oracle.ManagedDataAccess.Core --version 23.5.0
# EF Core 도구
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.5
cd ..

# appsettings.json 설정 파일 생성
echo "Creating Oracle connection configuration..."
cat > "$PROJECT_NAME.API/appsettings.json" << 'EOL'
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost:1521/XE;User Id=mes;Password=mes123;",
    "OracleConnection": "Data Source=localhost:1521/XE;User Id=mes;Password=mes123;"
  },
  "Jwt": {
    "Key": "MES_JWT_SECRET_KEY_FOR_ORACLE_PROJECT_2024",
    "Issuer": "OracleMES.API",
    "Audience": "OracleMES.Client",
    "ExpireMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Oracle": "Information"
    }
  },
  "AllowedHosts": "*"
}
EOL

# README.md 파일 생성
echo "Creating README file..."
cat > "README.md" << 'EOL'
# Oracle MES (Manufacturing Execution System)

Oracle 데이터베이스 기반의 제조 실행 시스템 API

## 프로젝트 구조

```
OracleMES/
├── OracleMES.API/           # Web API 계층
├── OracleMES.Core/          # 도메인 모델 및 인터페이스
├── OracleMES.Infrastructure/ # 데이터 액세스 계층
└── OracleMES.Tests/         # 단위 테스트
```

## 기술 스택

- **.NET 8** - 웹 API 프레임워크
- **Oracle Database XE** - 데이터베이스
- **Entity Framework Core** - ORM
- **AutoMapper** - 객체 매핑
- **FluentValidation** - 입력 검증
- **JWT** - 인증/인가
- **SignalR** - 실시간 통신
- **Swagger** - API 문서화

## 주요 기능

- 📋 작업지시서 관리
- 🏭 생산 실적 추적
- 📊 OEE 분석
- 🔍 품질 관리
- ⚡ 실시간 모니터링

## 실행 방법

1. Oracle 컨테이너 실행
```bash
docker run -d --name oracle-mes -p 1521:1521 -e ORACLE_PASSWORD=MesProject123! gvenzl/oracle-xe:21-slim
```

2. 프로젝트 실행
```bash
dotnet run --project OracleMES.API
```

3. Swagger UI 접속: https://localhost:5001/swagger
EOL

echo ""
echo "🎉 Oracle MES Project setup completed!"
echo ""
echo "📁 프로젝트 구조:"
echo "   OracleMES.API/           - Web API 계층"
echo "   OracleMES.Core/          - 도메인 모델"
echo "   OracleMES.Infrastructure/ - 데이터 액세스"
echo "   OracleMES.Tests/         - 테스트"
echo ""
echo "🔗 다음 단계:"
echo "1. cd OracleMES.API"
echo "2. Oracle 테이블에서 모델 생성:"
echo "   dotnet ef dbcontext scaffold "Data Source=localhost:1521/XE;User Id=mes;Password=mes123;" Oracle.EntityFrameworkCore -o Entities -c MesDbContext --context-dir Data --force"
echo "3. dotnet run"
echo ""
echo "🌐 API 문서: https://localhost:5001/swagger"
echo "📊 Oracle 연결: localhost:1521/XE (mes/mes123)"
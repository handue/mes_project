# Oracle MES (Manufacturing Execution System)
# Oracle MES (제조 실행 시스템)

Oracle 데이터베이스 기반의 제조 실행 시스템 API
Oracle Database-based Manufacturing Execution System API

## 프로젝트 구조
## Project Structure

```
OracleMES/
├── OracleMES.API/           # Web API 계층
├── OracleMES.Core/          # 도메인 모델 및 인터페이스
├── OracleMES.Infrastructure/ # 데이터 액세스 계층
└── OracleMES.Tests/         # 단위 테스트
```

```
OracleMES/
├── OracleMES.API/           # Web API Layer
├── OracleMES.Core/          # Domain Models & Interfaces
├── OracleMES.Infrastructure/ # Data Access Layer
└── OracleMES.Tests/         # Unit Tests
```

## 기술 스택
## Technology Stack

- **.NET 8** - 웹 API 프레임워크
- **Oracle Database XE** - 데이터베이스
- **Entity Framework Core** - ORM
- **AutoMapper** - 객체 매핑
- **FluentValidation** - 입력 검증
- **JWT** - 인증/인가
- **SignalR** - 실시간 통신
- **Swagger** - API 문서화

- **.NET 8** - Web API Framework
- **Oracle Database XE** - Database
- **Entity Framework Core** - ORM
- **AutoMapper** - Object Mapping
- **FluentValidation** - Input Validation
- **JWT** - Authentication/Authorization
- **SignalR** - Real-time Communication
- **Swagger** - API Documentation

## 주요 기능
## Key Features

- 📋 작업지시서 관리
- 🏭 생산 실적 추적
- 📊 OEE 분석
- 🔍 품질 관리
- ⚡ 실시간 모니터링

- 📋 Work Order Management
- 🏭 Production Performance Tracking
- 📊 OEE Analysis
- 🔍 Quality Management
- ⚡ Real-time Monitoring

## 실행 방법
## How to Run

1. Oracle 컨테이너 실행
1. Run Oracle Container
```bash
docker run -d --name oracle-mes -p 1521:1521 -e ORACLE_PASSWORD=MesProject123! gvenzl/oracle-xe:21-slim
```

2. 프로젝트 실행
2. Run Project
```bash
dotnet run --project OracleMES.API
```

3. Swagger UI 접속: https://localhost:5001/swagger
3. Access Swagger UI: https://localhost:5001/swagger

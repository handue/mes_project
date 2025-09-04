# Oracle MES (Manufacturing Execution System)
# Oracle MES (제조 실행 시스템)

![Oracle](https://img.shields.io/badge/Oracle-F80000?style=for-the-badge&logo=oracle&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

Oracle 데이터베이스 기반의 제조 실행 시스템으로, 실시간 생산 모니터링과 스마트 팩토리 운영을 지원합니다.  
Oracle Database-based Manufacturing Execution System supporting real-time production monitoring and smart factory operations.

## 📁 프로젝트 구조 | Project Structure

```
mes_project/
├── server/                          # 백엔드 서버 | Backend Server
│   ├── OracleMES.API/              # Web API 계층 | Web API Layer
│   │   ├── Controllers/            # API 컨트롤러 | API Controllers
│   │   ├── Program.cs              # 앱 진입점 | App Entry Point
│   │   └── appsettings.json        # 설정 파일 | Configuration
│   ├── OracleMES.Core/             # 도메인 모델 및 인터페이스 | Domain Models & Interfaces
│   │   ├── Interfaces/             # Repository 인터페이스 | Repository Interfaces
│   │   ├── DTOs/                   # 데이터 전송 객체 | Data Transfer Objects
│   │   └── Services/               # 비즈니스 서비스 | Business Services
│   ├── OracleMES.Infrastructure/   # 데이터 액세스 계층 | Data Access Layer
│   │   ├── Data/                   # DbContext | Database Context
│   │   ├── Entities/               # EF Core 엔티티 | EF Core Entities
│   │   └── Repositories/           # Repository 구현 | Repository Implementations
│   └── OracleMES.Tests/            # 단위 테스트 | Unit Tests
├── client/                         # 프론트엔드 클라이언트 | Frontend Client (계획됨 | Planned)
│   ├── src/                        # React 소스 코드 | React Source Code
│   ├── public/                     # 정적 파일 | Static Files
│   └── package.json                # NPM 의존성 | NPM Dependencies
├── docs/                           # 프로젝트 문서 | Project Documentation
└── README.md                       # 프로젝트 개요 | Project Overview
```

## 🚀 기술 스택 | Technology Stack

### 백엔드 | Backend
| 기술 | 버전 | 용도 | Technology | Version | Purpose |
|------|------|------|------------|---------|---------|
| .NET | 8.0 | 웹 API 프레임워크 | Web API Framework |
| Oracle Database XE | 21c | 메인 데이터베이스 | Main Database |
| Entity Framework Core | 8.0 | ORM | Object-Relational Mapping |
| AutoMapper | 12.0 | 객체 매핑 | Object Mapping |
| FluentValidation | 11.3 | 입력 검증 | Input Validation |
| JWT Bearer | 8.0 | 인증/인가 | Authentication/Authorization |
| SignalR | 1.1 | 실시간 통신 | Real-time Communication |
| Swagger/OpenAPI | 6.5 | API 문서화 | API Documentation |

### 프론트엔드 | Frontend (계획됨 | Planned)
| 기술 | 버전 | 용도 | Technology | Version | Purpose |
|------|------|------|------------|---------|---------|
| React | 18+ | UI 프레임워크 | UI Framework |
| TypeScript | 5+ | 타입 안전성 | Type Safety |
| Material-UI | 5+ | UI 컴포넌트 | UI Components |
| Recharts | 2+ | 차트 라이브러리 | Chart Library |
| SignalR Client | - | 실시간 통신 | Real-time Communication |

### 인프라 | Infrastructure
| 기술 | 버전 | 용도 | Technology | Version | Purpose |
|------|------|------|------------|---------|---------|
| Docker | - | 컨테이너화 | Containerization |
| Oracle XE Container | 21-slim | 데이터베이스 실행 | Database Runtime |

## ✨ 주요 기능 | Key Features

### 🏭 제조 실행 관리 | Manufacturing Execution Management
- 📋 **작업지시서 관리** | Work Order Management
  - 작업지시서 생성, 수정, 삭제 | Create, update, delete work orders
  - 실시간 상태 추적 (계획됨/진행중/완료/취소) | Real-time status tracking (Planned/InProgress/Completed/Cancelled)
  - 우선순위 및 일정 관리 | Priority and schedule management

- 🏭 **생산 실적 추적** | Production Performance Tracking
  - 실시간 생산량 기록 | Real-time production quantity recording
  - 자재 소비 추적 | Material consumption tracking
  - 작업센터별 성과 분석 | Work center performance analysis

### 📊 데이터 분석 | Data Analytics
- 📈 **OEE (Overall Equipment Effectiveness) 분석** | OEE Analysis
  - 가용성(Availability) 계산 | Availability calculation
  - 성능율(Performance) 측정 | Performance measurement
  - 품질률(Quality) 분석 | Quality rate analysis

- 🔍 **품질 관리** | Quality Management
  - 품질 검사 결과 기록 | Quality inspection result recording
  - 불량 유형 및 원인 분석 | Defect type and root cause analysis
  - 품질 트렌드 모니터링 | Quality trend monitoring

### ⚡ 실시간 모니터링 | Real-time Monitoring
- 🖥️ **실시간 대시보드** | Real-time Dashboard
  - 생산 현황 실시간 업데이트 | Real-time production status updates
  - 설비 상태 모니터링 | Equipment status monitoring
  - 알람 및 알림 시스템 | Alert and notification system

## 🛠️ 설치 및 실행 | Installation & Setup

### 사전 요구사항 | Prerequisites
- Docker Desktop
- .NET 8 SDK
- Git

### 1. 프로젝트 클론 | Clone Project
```bash
git clone <repository-url>
cd mes_project
```

### 2. Oracle 데이터베이스 실행 | Run Oracle Database
```bash
# Oracle XE 컨테이너 실행 | Run Oracle XE Container
docker run -d \
  --name oracle-mes \
  -p 1521:1521 \
  -e ORACLE_PASSWORD=MesProject123! \
  gvenzl/oracle-xe:21-slim

# 컨테이너 상태 확인 | Check container status
docker logs oracle-mes
# "DATABASE IS READY TO USE!" 메시지 확인 | Wait for "DATABASE IS READY TO USE!" message
```

### 3. 백엔드 실행 | Run Backend
```bash
cd server
dotnet restore
dotnet run --project OracleMES.API
```

### 4. API 문서 확인 | Check API Documentation
브라우저에서 접속 | Open in browser: https://localhost:5001/swagger

### 5. 프론트엔드 실행 | Run Frontend (향후 구현 | Future Implementation)
```bash
cd client
npm install
npm start
```

## 📊 데이터베이스 정보 | Database Information

### Oracle 연결 정보 | Oracle Connection Info
```
Host: localhost
Port: 1521
Database: XE
Username: mes
Password: mes123
Schema: MES
```

### 주요 테이블 | Main Tables
| 테이블명 | 용도 | 레코드 수 | Table Name | Purpose | Record Count |
|----------|------|-----------|------------|---------|--------------|
| WORKORDERS | 작업지시서 | ~640K | Work Orders | ~640K |
| PRODUCTS | 제품 마스터 | ~100 | Product Master | ~100 |
| MACHINES | 설비 정보 | ~50 | Equipment Info | ~50 |
| EMPLOYEES | 직원 정보 | ~200 | Employee Info | ~200 |
| QUALITYCONTROL | 품질 관리 | ~640K | Quality Control | ~640K |
| OEEMETRICS | OEE 지표 | ~64K | OEE Metrics | ~64K |
| MATERIALCONSUMPTION | 자재 소비 | ~2M | Material Consumption | ~2M |

## 🔗 API 엔드포인트 | API Endpoints

### 작업지시서 관리 | Work Order Management
```http
GET    /api/workorders           # 작업지시서 목록 조회 | Get work order list
GET    /api/workorders/{id}      # 특정 작업지시서 조회 | Get specific work order
POST   /api/workorders           # 새 작업지시서 생성 | Create new work order
PUT    /api/workorders/{id}      # 작업지시서 수정 | Update work order
DELETE /api/workorders/{id}      # 작업지시서 삭제 | Delete work order
POST   /api/workorders/{id}/start    # 작업 시작 | Start work order
POST   /api/workorders/{id}/complete # 작업 완료 | Complete work order
```

### 생산 실적 | Production Records
```http
GET    /api/production-records   # 생산 실적 조회 | Get production records
POST   /api/production-records   # 생산 실적 입력 | Record production
```

### OEE 분석 | OEE Analysis
```http
GET    /api/oee/daily           # 일별 OEE | Daily OEE
GET    /api/oee/machines/{id}   # 설비별 OEE | Machine-specific OEE
```

### 품질 관리 | Quality Management
```http
GET    /api/quality-control     # 품질 검사 이력 | Quality inspection history
POST   /api/quality-control     # 검사 결과 입력 | Record inspection result
```

## 🧪 개발 및 테스트 | Development & Testing

### 백엔드 테스트 | Backend Testing
```bash
# 단위 테스트 실행 | Run unit tests
cd server
dotnet test

# 특정 테스트 실행 | Run specific tests
dotnet test --filter "WorkOrderTests"
```

### API 테스트 도구 | API Testing Tools
- **Swagger UI**: https://localhost:5001/swagger
- **Postman**: API 컬렉션 임포트 | Import API collection
- **curl**: 명령행 테스트 | Command line testing

## 📈 성능 지표 | Performance Metrics

### 현재 데이터 규모 | Current Data Scale
- 📊 **640K+ 작업지시서** | 640K+ Work Orders
- 🏭 **2M+ 자재 소비 레코드** | 2M+ Material Consumption Records
- 📋 **640K+ 품질 검사 기록** | 640K+ Quality Control Records
- ⚡ **실시간 처리** | Real-time Processing

### 목표 성능 | Target Performance
- API 응답 시간 < 200ms | API Response Time < 200ms
- 동시 접속자 100+ | Concurrent Users 100+
- 실시간 업데이트 지연 < 1초 | Real-time Update Latency < 1s

## 🤝 깃 업로드 방법 | Contributing

### 🌿 Git 브랜치 전략 | Git Branch Strategy

#### 기본 브랜치 구조 | Basic Branch Structure

```
main (또는 master) # 프로덕션 배포용 | Production deployment
├── develop # 개발 통합용 | Development integration
│ ├── feature/* # 새로운 기능 개발 | New feature development
│ ├── bugfix/* # 버그 수정 | Bug fixes
│ └── hotfix/* # 긴급 수정 | Emergency fixes
└── release/* # 릴리즈 준비 | Release preparation
```


#### 브랜치별 역할 | Branch Roles

| 브랜치명 | 용도 | 설명 | Branch Name | Purpose | Description |
|----------|------|------|-------------|---------|-------------|
| `main` | 프로덕션 배포 | 안정적인 릴리즈 버전 | Production deployment | Stable release version |
| `develop` | 개발 통합 | 다음 릴리즈 준비 | Development integration | Next release preparation |
| `feature/*` | 기능 개발 | 새로운 기능 개발 | Feature development | New feature development |
| `bugfix/*` | 버그 수정 | 버그 수정 작업 | Bug fixes | Bug fix work |
| `hotfix/*` | 긴급 수정 | 긴급 수정 사항 | Emergency fixes | Emergency fixes |
| `release/*` | 릴리즈 준비 | 릴리즈 전 최종 테스트 | Release preparation | Final testing before release |

#### 브랜치 네이밍 컨벤션 | Branch Naming Convention
```bash
feature/기능명-세부사항          # 예: feature/workorder-crud-api
bugfix/버그명-수정내용          # 예: bugfix/workorder-status-update
hotfix/긴급수정내용            # 예: hotfix/database-connection-issue
release/버전번호               # 예: release/v1.0.0
```

#### 개발 워크플로우 | Development Workflow

##### 1. 새로운 기능 개발 | New Feature Development
```bash
# develop 브랜치에서 시작
git checkout -b develop (creation)
git checkout develop
git pull origin develop

# feature 브랜치 생성
git checkout -b feature/workorder-crud-api

# 개발 작업
# ... 코드 작성 ...
git add .
git commit -m "작업지시서 CRUD API 구현"

# 원격 저장소에 푸시
git push origin feature/workorder-crud-api

# Pull Request 생성 (GitHub/GitLab에서)
# develop 브랜치로 머지 요청

# 코드 리뷰 후 머지 완료 시
git checkout develop
git pull origin develop
git branch -d feature/workorder-crud-api
```

##### 2. 버그 수정 | Bug Fix
```bash
# develop 브랜치에서 시작
git checkout develop
git pull origin develop

# bugfix 브랜치 생성
git checkout -b bugfix/workorder-status-update

# 수정 작업
# ... 버그 수정 ...
git add .
git commit -m "작업지시서 상태 업데이트 버그 수정"

# Pull Request 생성 후 develop에 머지
```

##### 3. 긴급 수정 | Hotfix
```bash
# main 브랜치에서 시작
git checkout main
git pull origin main

# hotfix 브랜치 생성
git checkout -b hotfix/security-patch

# 긴급 수정 작업
# ... 긴급 수정 ...
git add .
git commit -m "보안 패치 적용"

# main과 develop 모두에 머지
```

#### MES 프로젝트 브랜치 예시 | MES Project Branch Examples

```bash
# 작업지시서 관련 | Work Order Related
feature/workorder-crud-api
feature/workorder-status-tracking
feature/workorder-priority-management

# 생산 실적 관련 | Production Records Related
feature/production-records-api
feature/oee-calculation-logic
feature/real-time-monitoring

# 품질 관리 관련 | Quality Management Related
feature/quality-control-system
feature/defect-analysis
feature/quality-reports

# 사용자 관리 | User Management
feature/user-authentication
feature/role-based-access-control
```

#### 브랜치 관리 명령어 | Branch Management Commands

```bash
# 브랜치 목록 확인 | List all branches
git branch -a

# 현재 브랜치 확인 | Check current branch
git branch

# 브랜치 생성 및 이동 | Create and switch to branch
git checkout -b feature/새기능

# 브랜치 삭제 | Delete branch
git branch -d feature/완료된기능

# 원격 브랜치 삭제 | Delete remote branch
git push origin --delete feature/완료된기능

# 브랜치 강제 삭제 | Force delete branch
git branch -D feature/강제삭제할브랜치
```

#### 권장사항 | Best Practices

1. **항상 feature 브랜치 사용** | Always use feature branches
2. **작은 단위로 커밋** | Commit in small units
3. **의미있는 커밋 메시지 작성** | Write meaningful commit messages
4. **Pull Request로 코드 리뷰** | Code review through Pull Requests
5. **브랜치 정리** | Clean up branches after merge
6. **main 브랜치 보호** | Protect main branch
7. **정기적인 develop 머지** | Regular merges to develop

#### 커밋 메시지 컨벤션 | Commit Message Convention

```bash
feat: 새로운 기능 추가 | Add new feature
fix: 버그 수정 | Fix bug
docs: 문서 수정 | Update documentation
style: 코드 포맷팅 | Code formatting
refactor: 코드 리팩토링 | Code refactoring
test: 테스트 추가 | Add tests
chore: 빌드 프로세스 수정 | Update build process
```

#### 기존 Contributing 가이드 | Original Contributing Guide

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



## 📋 개발 로드맵 | Development Roadmap

### Phase 1: 백엔드 API 개발 | Backend API Development ✅
- [x] 프로젝트 구조 설정 | Project structure setup
- [x] Oracle DB 연동 | Oracle DB integration
- [x] Entity Framework 설정 | Entity Framework configuration
- [x] Repository 패턴 구현 | Repository pattern implementation
- [x] DI 컨테이너 설정 | Dependency injection setup
- [x] 예외 처리 및 로깅 | Exception handling and logging
- [x] **Controller API 개발 | Controller API Development** ✅
  - [x] WorkorderController (작업지시서 관리) | Work Order Management
  - [x] MachineController (설비 관리) | Machine Management
  - [x] EmployeeController (직원 관리) | Employee Management
  - [x] QualitycontrolController (품질 관리) | Quality Control
  - [x] InventoryController (재고 관리) | Inventory Management
  - [x] OEEController (OEE 분석) | OEE Analysis
  - [x] ProductController (제품 관리) | Product Management

### Phase 2: 비즈니스 로직 및 서비스 | Business Logic & Services ✅
- [x] 비즈니스 서비스 구현 | Business service implementation
  - [x] WorkorderService (작업지시서 비즈니스 로직) | Work Order Business Logic
  - [x] MachineService (설비 관리 비즈니스 로직) | Machine Management Business Logic
  - [x] EmployeeService (직원 관리 비즈니스 로직) | Employee Management Business Logic
  - [x] QualityControlService (품질 관리 비즈니스 로직) | Quality Control Business Logic
  - [x] InventoryService (재고 관리 비즈니스 로직) | Inventory Management Business Logic
  - [x] OEEService (OEE 분석 비즈니스 로직) | OEE Analysis Business Logic
  - [x] ProductService (제품 관리 비즈니스 로직) | Product Management Business Logic
  - [x] MaterialConsumptionService (자재 소비 비즈니스 로직) | Material Consumption Business Logic
  - [x] WorkcenterService (작업장 관리 비즈니스 로직) | Workcenter Management Business Logic
  - [x] DowntimeService (다운타임 관리 비즈니스 로직) | Downtime Management Business Logic
- [x] DTO 클래스 구현 | DTO classes implementation
- [x] 예외 처리 및 로깅 | Exception handling and logging
- [ ] **AutoMapper 설정 및 DTO 변환 로직** 🔄
  - [ ] AutoMapper 프로필 설정 | AutoMapper profile configuration
  - [ ] Entity ↔ DTO 변환 로직 | Entity ↔ DTO conversion logic
  - [ ] 서비스 메서드 DTO 인자 변경 | Service methods DTO parameter changes

### Phase 3: 인증 및 보안 | Authentication & Security
- [ ] JWT 인증 구현 | JWT authentication implementation
- [ ] 사용자 관리 API | User management API
- [ ] 역할 기반 권한 관리 | Role-based access control
- [ ] API 보안 강화 | API security enhancement

### Phase 4: 실시간 기능 | Real-time Features
- [ ] SignalR Hub 구현 | SignalR Hub implementation
- [ ] 실시간 생산 모니터링 | Real-time production monitoring
- [ ] 실시간 알림 시스템 | Real-time notification system
- [ ] WebSocket 연결 관리 | WebSocket connection management

### Phase 5: 프론트엔드 개발 | Frontend Development
- [ ] React 프로젝트 설정 | React project setup
- [ ] 대시보드 UI 구현 | Dashboard UI implementation
- [ ] 실시간 차트 및 그래프 | Real-time charts and graphs
- [ ] 모바일 반응형 디자인 | Mobile responsive design
- [ ] 사용자 인증 UI | User authentication UI

### Phase 6: 테스트 및 품질 | Testing & Quality
- [ ] 단위 테스트 작성 | Unit test writing
- [ ] 통합 테스트 구현 | Integration test implementation
- [ ] API 테스트 자동화 | API test automation
- [ ] 성능 테스트 | Performance testing

### Phase 7: 배포 및 최적화 | Deployment & Optimization
- [ ] Docker 컨테이너화 | Docker containerization
- [ ] CI/CD 파이프라인 구축 | CI/CD pipeline setup
- [ ] 데이터베이스 최적화 | Database optimization
- [ ] API 성능 최적화 | API performance optimization
- [ ] 보안 감사 및 강화 | Security audit and enhancement

### Phase 8: 고급 기능 | Advanced Features
- [ ] 리포팅 시스템 | Reporting system
- [ ] 데이터 분석 및 예측 | Data analytics and prediction
- [ ] 모바일 앱 개발 | Mobile app development
- [ ] 외부 시스템 연동 | External system integration



---

## 🏷️ 태그 | Tags

`#MES` `#Oracle` `#DotNet` `#React` `#Manufacturing` `#SmartFactory` `#IoT` `#RealTime`  
`#제조실행시스템` `#오라클` `#닷넷` `#리액트` `#제조업` `#스마트팩토리`

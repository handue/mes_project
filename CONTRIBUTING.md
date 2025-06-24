# Git Contributing Guide | Git 협업 가이드

이 문서는 Oracle MES 프로젝트에서 협업할 때 사용하는 Git 브랜치 전략과 커밋 방법을 간단하게 정리한 가이드입니다.  
This document is a simple guide for Git branch strategy and commit methods used in Oracle MES project collaboration.

---

## 1. 브랜치 전략 | Branch Strategy

- **main**: 운영/배포용(항상 안정적인 코드) | Production/Deployment (Always stable code)
- **develop**: 개발 통합(여러 기능 합치는 곳) | Development Integration (Where features are merged)
- **feature/**: 새로운 기능 개발 | New Feature Development
- **bugfix/**: 버그 수정 | Bug Fixes
- **hotfix/**: 긴급 수정 | Emergency Fixes

### 브랜치 생성 예시 | Branch Creation Example
```bash
git checkout develop
git pull origin develop
git checkout -b feature/기능명
```

---

## 2. 커밋 & 푸시 | Commit & Push

### 1) 변경사항 확인 | Check Changes
```bash
git status
git diff
```

### 2) 파일 추가 | Add Files
```bash
git add 파일명1 파일명2
# 또는 전체 추가 | Or add all
git add .
```

### 3) 커밋 (영어 권장) | Commit (English recommended)
```bash
git commit -m "feat: Add user login API"
```

### 4) 원격 저장소에 푸시 | Push to Remote Repository
```bash
git push origin feature/기능명
```

---

## 3. Pull Request & 머지 | Pull Request & Merge

1. GitHub에서 Pull Request(PR) 생성 | Create Pull Request on GitHub
2. 코드 리뷰/테스트 후 develop 브랜치에 머지 | Merge to develop after code review/test
3. 여러 기능이 모이면 develop → main에 머지(배포) | Merge develop → main when features are ready (deployment)

---

## 4. 커밋 메시지 컨벤션 | Commit Message Convention
- `feat:` 새로운 기능 | New feature
- `fix:` 버그 수정 | Bug fix
- `docs:` 문서 | Documentation
- `refactor:` 리팩토링 | Refactoring
- `test:` 테스트 | Test
- `chore:` 기타 작업 | Other tasks

---

## 5. 자주 쓰는 명령어 | Frequently Used Commands
```bash
git branch -a          # 브랜치 목록 | List branches
git checkout 브랜치명   # 브랜치 이동 | Switch branch
git pull origin 브랜치명 # 원격 브랜치 최신화 | Update remote branch
git push origin 브랜치명 # 원격 브랜치 푸시 | Push to remote branch
git branch -d 브랜치명   # 로컬 브랜치 삭제 | Delete local branch
git push origin --delete 브랜치명 # 원격 브랜치 삭제 | Delete remote branch
```

---

## 6. GitHub Activity 기록 | GitHub Activity Recording

### Activity가 기록되는 경우 | When Activity is Recorded
- **main 브랜치에 머지된 커밋** | Commits merged to main branch
- **Pull Request로 머지된 커밋** | Commits merged via Pull Request
- **default 브랜치(main)에 직접 푸시된 커밋** | Commits directly pushed to default branch (main)

### Activity 기록 방법 | How to Record Activity
```bash
# 방법 1: develop에서 개발 후 main에 머지
# Method 1: Develop on develop branch then merge to main
git checkout develop
git checkout -b feature/new-feature
# ... 개발 작업 | ... development work
git add .
git commit -m "feat: Add new feature"
git push origin feature/new-feature
# GitHub에서 Pull Request 생성 후 main에 머지
# Create Pull Request on GitHub then merge to main

# 방법 2: develop에서 개발 후 직접 main에 머지
# Method 2: Develop on develop branch then merge directly to main
git checkout develop
# ... 개발 작업 | ... development work
git add .
git commit -m "feat: Add new feature"
git push origin develop
git checkout main
git merge develop
git push origin main
```

### 주의사항 | Important Notes
- **develop에서만 작업하고 main에 머지하지 않으면** Activity에 기록되지 않을 수 있음
- **main에 머지된 커밋만** GitHub Contributions(잔디)에 확실히 기록됨
- **여러 커밋을 develop에서 했다면, main에 머지할 때 모든 커밋이 Activity에 기록됨**

---

## 7. 참고 | References
- 항상 작은 단위로 커밋하세요. | Always commit in small units.
- 의미 있는 커밋 메시지를 작성하세요. | Write meaningful commit messages.
- main에는 반드시 Pull Request로만 머지하세요. | Always merge to main via Pull Request.
- 궁금한 점은 README.md와 이 문서를 참고하세요. | Refer to README.md and this document for questions. 
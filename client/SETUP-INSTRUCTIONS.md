# React + Vite + TypeScript + Tailwind CSS 설정 가이드 | Setup Guide

## 🚀 프로젝트 설정 순서 | Project Setup Steps

### 1. React 프로젝트 생성 | Create React Project

```bash
# Vite를 사용한 React TypeScript 프로젝트 생성
# Create React TypeScript project using Vite
npm create vite@latest client -- --template react-ts
cd client
npm install
```

### 2. Tailwind CSS 설치 | Install Tailwind CSS

```bash
# Tailwind CSS v3 (안정 버전) 설치
# Install Tailwind CSS v3 (stable version)
npm install -D tailwindcss@3 postcss autoprefixer

# 또는 최신 버전 (v4는 베타이므로 주의)
# Or latest version (note: v4 is beta, use with caution)
# npm install -D tailwindcss@latest postcss autoprefixer
```

⚠️ **주의사항 | Warning**: Tailwind CSS v4는 아직 베타 버전이므로 프로덕션에서는 v3 사용 권장 | Tailwind CSS v4 is still in beta, recommend using v3 for production

### 3. 설정 파일 생성 | Create Configuration Files

#### 3.1 Tailwind 설정 파일 | Tailwind Config File (`tailwind.config.js`)

```javascript
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
```

#### 3.2 PostCSS 설정 파일 | PostCSS Config File (`postcss.config.js`)

```javascript
export default {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  },
}
```

⚠️ **ES Module 주의사항 | ES Module Note**: `package.json`에 `"type": "module"`이 있으면 반드시 `export default` 문법 사용 | If `"type": "module"` exists in `package.json`, must use `export default` syntax

### 4. CSS 파일 설정 | CSS File Setup (`src/index.css`)

```css
@tailwind base;
@tailwind components;
@tailwind utilities;

:root {
  font-family: system-ui, Avenir, Helvetica, Arial, sans-serif;
  line-height: 1.5;
  font-weight: 400;
}

body {
  margin: 0;
  min-width: 320px;
  min-height: 100vh;
}
```

### 5. 추가 라이브러리 설치 | Install Additional Libraries

```bash
# API 통신 | API Communication
npm install axios

# 아이콘 | Icons
npm install lucide-react

# 상태관리 (선택사항) | State Management (Optional)
npm install @reduxjs/toolkit react-redux
```

## 🔧 일반적인 문제 해결 | Common Troubleshooting

### 문제 1: PostCSS 설정 오류 | Issue 1: PostCSS Configuration Error

**오류 | Error**: `module is not defined in ES module scope`

**해결방법 | Solution**:
```javascript
// ❌ 잘못된 방법 (CommonJS) | Wrong way (CommonJS)
module.exports = {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  },
}

// ✅ 올바른 방법 (ES Module) | Correct way (ES Module)
export default {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  },
}
```

### 문제 2: Tailwind CSS v4 설정 충돌 | Issue 2: Tailwind CSS v4 Configuration Conflict

**오류 | Error**: `tailwindcss directly as a PostCSS plugin`

**원인 | Cause**: Tailwind CSS v4는 다른 설정 방식 사용 | Tailwind CSS v4 uses different configuration approach

**해결방법 | Solution**:
```bash
# v4 제거하고 v3 설치 | Remove v4 and install v3
npm uninstall tailwindcss @tailwindcss/postcss
npm install -D tailwindcss@3
```

### 문제 3: Tailwind 스타일이 적용되지 않음 | Issue 3: Tailwind Styles Not Applied

**체크리스트 | Checklist**:
1. `src/index.css`에 `@tailwind` 지시어 확인 | Check `@tailwind` directives in `src/index.css`
2. `main.tsx`에서 `import './index.css'` 확인 | Check `import './index.css'` in `main.tsx`
3. `tailwind.config.js`의 `content` 경로 확인 | Check `content` paths in `tailwind.config.js`
4. 개발 서버 재시작 | Restart development server

## 📦 패키지 버전 호환성 | Package Version Compatibility

### 추천 버전 조합 | Recommended Version Combination

```json
{
  "dependencies": {
    "react": "^19.1.1",
    "react-dom": "^19.1.1"
  },
  "devDependencies": {
    "tailwindcss": "^3.4.0",
    "postcss": "^8.5.6",
    "autoprefixer": "^10.4.21",
    "vite": "^7.1.0",
    "typescript": "~5.8.3"
  }
}
```

### 버전별 주요 차이점 | Major Differences by Version

| 버전 \| Version | 설정 방식 \| Configuration | 안정성 \| Stability | 권장도 \| Recommendation |
|------|-----------|--------|--------|
| Tailwind v3 | 기존 PostCSS 플러그인 \| Traditional PostCSS plugin | ✅ 안정 \| Stable | ⭐⭐⭐ |
| Tailwind v4 | `@tailwindcss/postcss` | ⚠️ 베타 \| Beta | ⭐ |

## 🎨 프로젝트별 커스터마이징 | Project Customization

### 테마 확장 예시 | Theme Extension Example

```javascript
// tailwind.config.js
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          500: '#3b82f6',
          900: '#1e3a8a',
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
```

### 플러그인 추가 | Adding Plugins

```bash
# 유용한 Tailwind 플러그인들 | Useful Tailwind plugins
npm install -D @tailwindcss/forms
npm install -D @tailwindcss/typography
npm install -D @tailwindcss/aspect-ratio
```

```javascript
// tailwind.config.js
export default {
  // ... 기존 설정 | existing configuration
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
  ],
}
```

## 🧪 설정 검증 | Configuration Verification

### 1. 기본 테스트 | Basic Test

```tsx
// src/App.tsx
function App() {
  return (
    <div className="min-h-screen bg-gray-100">
      <div className="container mx-auto py-8">
        <h1 className="text-4xl font-bold text-blue-600">
          Tailwind CSS 작동 확인 | Tailwind CSS Working Check
        </h1>
        <p className="mt-4 text-gray-700">
          이 텍스트가 스타일링되어 보이면 성공! | Success if this text appears styled!
        </p>
      </div>
    </div>
  )
}
```

### 2. 빌드 테스트 | Build Test

```bash
# 개발 서버 | Development server
npm run dev

# 프로덕션 빌드 | Production build
npm run build

# 빌드 미리보기 | Build preview
npm run preview
```

## 📚 추가 리소스 | Additional Resources

- [Tailwind CSS 공식 문서 | Official Documentation](https://tailwindcss.com/docs)
- [Vite 공식 문서 | Official Documentation](https://vitejs.dev/)
- [PostCSS 플러그인 가이드 | Plugin Guide](https://postcss.org/)

## 🆘 트러블슈팅 | Troubleshooting

### 자주 발생하는 오류들 | Common Errors

1. **CSS 적용 안됨 | CSS Not Applied**: 개발 서버 재시작 필요 | Development server restart required
2. **모듈 오류 | Module Error**: ES Module vs CommonJS 문법 확인 | Check ES Module vs CommonJS syntax
3. **버전 충돌 | Version Conflict**: 패키지 버전 호환성 확인 | Check package version compatibility
4. **빌드 실패 | Build Failure**: TypeScript 타입 오류 해결 | Resolve TypeScript type errors

### 도움이 되는 명령어 | Helpful Commands

```bash
# 캐시 정리 | Clear cache
npm run dev -- --force

# 노드 모듈 재설치 | Reinstall node modules
rm -rf node_modules package-lock.json
npm install

# Tailwind 설정 검증 | Verify Tailwind configuration
npx tailwindcss --help
```


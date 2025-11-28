# Test Execution Guide

This guide provides comprehensive instructions for running the GloboTicket Playwright test suite in various configurations and scenarios.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Basic Test Execution](#basic-test-execution)
- [Running Specific Tests](#running-specific-tests)
- [Browser Configuration](#browser-configuration)
- [Display Modes](#display-modes)
- [Debugging Tests](#debugging-tests)
- [Test Reports](#test-reports)
- [Environment Configuration](#environment-configuration)
- [CI/CD Integration](#cicd-integration)
- [Advanced Options](#advanced-options)
- [Performance Tips](#performance-tips)

## Prerequisites

### Required Software
- Node.js (v18 or higher)
- npm (v9 or higher)
- Playwright browsers installed

### Installation

1. **Install dependencies**:
```bash
cd src/GloboTicket.Web
npm install
```

2. **Install Playwright browsers**:
```bash
npx playwright install
```

3. **Install browser dependencies** (Linux only):
```bash
npx playwright install-deps
```

### Backend Requirements
- The GloboTicket API must be running on `http://localhost:5000` (or configured BASE_URL)
- Database must be seeded with test user credentials:
  - Username: `prod`
  - Password: `prod123`

## Basic Test Execution

### Run All Tests
Execute the entire test suite:
```bash
npm run test:e2e
```

Or using Playwright directly:
```bash
npx playwright test
```

### Run Tests with Output
Show test output in the console:
```bash
npx playwright test --reporter=list
```

### Run Tests in Parallel
Run tests across multiple workers (default):
```bash
npx playwright test --workers=4
```

### Run Tests Sequentially
Run tests one at a time:
```bash
npx playwright test --workers=1
```

## Running Specific Tests

### Run a Single Test File
```bash
npx playwright test tests/auth/login.spec.ts
```

### Run Multiple Test Files
```bash
npx playwright test tests/auth/ tests/venues/
```

### Run Tests by Name Pattern
Run tests matching a specific pattern:
```bash
npx playwright test -g "should successfully log in"
```

### Run Tests by Tag
If you've added tags to your tests:
```bash
npx playwright test --grep @smoke
```

### Skip Specific Tests
```bash
npx playwright test --grep-invert @slow
```

### Run Only Failed Tests
Re-run tests that failed in the last run:
```bash
npx playwright test --last-failed
```

## Browser Configuration

### Run in Specific Browser

**Chromium** (default):
```bash
npx playwright test --project=chromium
```

**Firefox**:
```bash
npx playwright test --project=firefox
```

**WebKit** (Safari):
```bash
npx playwright test --project=webkit
```

### Run in All Browsers
```bash
npx playwright test --project=chromium --project=firefox --project=webkit
```

### Run in Mobile Browsers
First, uncomment mobile configurations in [`playwright.config.ts`](../playwright.config.ts), then:

```bash
npx playwright test --project="Mobile Chrome"
npx playwright test --project="Mobile Safari"
```

### Run in Branded Browsers
First, uncomment branded browser configurations in [`playwright.config.ts`](../playwright.config.ts), then:

```bash
npx playwright test --project="Microsoft Edge"
npx playwright test --project="Google Chrome"
```

## Display Modes

### Headless Mode (Default)
Tests run without visible browser window:
```bash
npx playwright test
```

### Headed Mode
Watch tests execute in a visible browser:
```bash
npx playwright test --headed
```

### UI Mode
Interactive mode with test explorer and time-travel debugging:
```bash
npx playwright test --ui
```

Features of UI mode:
- Visual test explorer
- Watch mode (auto-rerun on file changes)
- Time-travel debugging
- Network inspection
- Console logs
- Screenshots and videos

### Slow Motion
Run tests in slow motion (useful for debugging):
```bash
npx playwright test --headed --slow-mo=1000
```

## Debugging Tests

### Debug Mode
Opens Playwright Inspector for step-by-step debugging:
```bash
npx playwright test --debug
```

Features:
- Step through test execution
- Inspect page state
- View console logs
- Modify selectors in real-time
- Record test actions

### Debug Specific Test
```bash
npx playwright test tests/auth/login.spec.ts --debug
```

### Debug from Specific Line
Add `await page.pause()` in your test code:
```typescript
test('my test', async ({ page }) => {
  await page.goto('/login');
  await page.pause(); // Execution pauses here
  // ... rest of test
});
```

### Visual Studio Code Debugging

1. Install the Playwright Test for VSCode extension
2. Set breakpoints in your test files
3. Click "Debug Test" in the test explorer or code lens

### Browser Developer Tools
Run with browser devtools open:
```bash
PWDEBUG=console npx playwright test --headed
```

## Test Reports

### HTML Report (Default)
After test execution, view the HTML report:
```bash
npx playwright show-report
```

The report includes:
- Test results summary
- Failed test details
- Screenshots of failures
- Test traces
- Execution timeline

### Generate Report Without Running Tests
```bash
npx playwright show-report playwright-report
```

### Other Report Formats

**List Reporter** (console output):
```bash
npx playwright test --reporter=list
```

**Line Reporter** (minimal output):
```bash
npx playwright test --reporter=line
```

**JSON Reporter**:
```bash
npx playwright test --reporter=json
```

**JUnit Reporter** (for CI):
```bash
npx playwright test --reporter=junit
```

**Multiple Reporters**:
```bash
npx playwright test --reporter=list --reporter=html --reporter=json
```

### Trace Viewer
View detailed traces of test execution:
```bash
npx playwright show-trace trace.zip
```

Features:
- Action timeline
- Network requests
- Console logs
- Screenshots at each step
- Source code
- Call stack

## Environment Configuration

### Set Base URL
Override the default base URL:

**Linux/Mac**:
```bash
BASE_URL=http://localhost:3000 npx playwright test
```

**Windows (PowerShell)**:
```powershell
$env:BASE_URL="http://localhost:3000"; npx playwright test
```

**Windows (CMD)**:
```cmd
set BASE_URL=http://localhost:3000 && npx playwright test
```

### Using .env File
Create a `.env` file in the project root:
```env
BASE_URL=http://localhost:5173
```

Then uncomment the dotenv configuration in [`playwright.config.ts`](../playwright.config.ts).

### Test Against Different Environments

**Development**:
```bash
BASE_URL=http://localhost:5173 npx playwright test
```

**Staging**:
```bash
BASE_URL=https://staging.globoticket.com npx playwright test
```

**Production** (use with caution):
```bash
BASE_URL=https://globoticket.com npx playwright test
```

## CI/CD Integration

### GitHub Actions

Create `.github/workflows/playwright.yml`:

```yaml
name: Playwright Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  test:
    timeout-minutes: 60
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - uses: actions/setup-node@v3
      with:
        node-version: 18
        
    - name: Install dependencies
      run: |
        cd src/GloboTicket.Web
        npm ci
        
    - name: Install Playwright Browsers
      run: |
        cd src/GloboTicket.Web
        npx playwright install --with-deps
        
    - name: Run Playwright tests
      run: |
        cd src/GloboTicket.Web
        npx playwright test
        
    - uses: actions/upload-artifact@v3
      if: always()
      with:
        name: playwright-report
        path: src/GloboTicket.Web/playwright-report/
        retention-days: 30
```

### GitLab CI

Create `.gitlab-ci.yml`:

```yaml
test:
  image: mcr.microsoft.com/playwright:v1.40.0-focal
  script:
    - cd src/GloboTicket.Web
    - npm ci
    - npx playwright test
  artifacts:
    when: always
    paths:
      - src/GloboTicket.Web/playwright-report/
    expire_in: 1 week
```

### Azure Pipelines

Create `azure-pipelines.yml`:

```yaml
trigger:
  - main
  - develop

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: NodeTool@0
  inputs:
    versionSpec: '18.x'
    
- script: |
    cd src/GloboTicket.Web
    npm ci
    npx playwright install --with-deps
  displayName: 'Install dependencies'
  
- script: |
    cd src/GloboTicket.Web
    npx playwright test
  displayName: 'Run Playwright tests'
  
- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'JUnit'
    testResultsFiles: '**/test-results/junit.xml'
  condition: always()
```

### Docker

Create a `Dockerfile` for running tests:

```dockerfile
FROM mcr.microsoft.com/playwright:v1.40.0-focal

WORKDIR /app

COPY src/GloboTicket.Web/package*.json ./
RUN npm ci

COPY src/GloboTicket.Web/ ./

CMD ["npx", "playwright", "test"]
```

Run tests in Docker:
```bash
docker build -t globoticket-tests .
docker run --rm globoticket-tests
```

## Advanced Options

### Timeout Configuration

**Global timeout** (in playwright.config.ts):
```typescript
timeout: 30000, // 30 seconds
```

**Per-test timeout**:
```typescript
test('my test', async ({ page }) => {
  test.setTimeout(60000); // 60 seconds
  // ... test code
});
```

**Command line**:
```bash
npx playwright test --timeout=60000
```

### Retry Configuration

**Retry failed tests**:
```bash
npx playwright test --retries=2
```

**Retry only on CI**:
```bash
npx playwright test --retries=2 --grep-invert @no-retry
```

### Update Snapshots
If you have visual regression tests:
```bash
npx playwright test --update-snapshots
```

### Ignore HTTPS Errors
```bash
npx playwright test --ignore-https-errors
```

### Maximum Failures
Stop after N test failures:
```bash
npx playwright test --max-failures=5
```

### Shard Tests
Split tests across multiple machines:

**Machine 1**:
```bash
npx playwright test --shard=1/3
```

**Machine 2**:
```bash
npx playwright test --shard=2/3
```

**Machine 3**:
```bash
npx playwright test --shard=3/3
```

## Performance Tips

### 1. Use Parallel Execution
```bash
npx playwright test --workers=4
```

### 2. Reuse Authentication State
The test suite already implements this via fixtures. Authentication state is saved to `playwright/.auth/user.json`.

### 3. Run Only Changed Tests
```bash
npx playwright test --only-changed
```

### 4. Use Test Filtering
Run only smoke tests during development:
```bash
npx playwright test -g @smoke
```

### 5. Optimize Wait Strategies
Use explicit waits instead of arbitrary timeouts:
```typescript
// Good
await page.waitForLoadState('networkidle');

// Avoid
await page.waitForTimeout(5000);
```

### 6. Run Tests in Headed Mode Selectively
Only use `--headed` when debugging specific tests.

### 7. Clear Test Data Periodically
Clean up test venues from the database to improve performance.

## Common Commands Reference

| Command | Description |
|---------|-------------|
| `npm run test:e2e` | Run all tests |
| `npx playwright test --ui` | Open UI mode |
| `npx playwright test --debug` | Debug mode |
| `npx playwright test --headed` | Headed mode |
| `npx playwright test --project=chromium` | Run in Chromium |
| `npx playwright test tests/auth/` | Run auth tests |
| `npx playwright test -g "login"` | Run tests matching "login" |
| `npx playwright show-report` | View HTML report |
| `npx playwright codegen` | Generate test code |
| `npx playwright test --last-failed` | Re-run failed tests |
| `npx playwright test --workers=1` | Sequential execution |

## Troubleshooting

### Tests Timeout
- Increase timeout: `--timeout=60000`
- Check if backend is running
- Verify network connectivity

### Browser Not Found
```bash
npx playwright install chromium
```

### Permission Denied (Linux)
```bash
sudo npx playwright install-deps
```

### Port Already in Use
Change BASE_URL or stop conflicting service:
```bash
lsof -ti:5173 | xargs kill -9
```

### Authentication Fails
Clear auth state:
```bash
rm -rf playwright/.auth
```

### Flaky Tests
- Use `waitForLoadState('networkidle')`
- Add explicit waits
- Ensure unique test data

## Getting Help

- [Playwright Documentation](https://playwright.dev)
- [TEST_SUITE.md](TEST_SUITE.md) - Test suite overview
- [MAINTENANCE_GUIDE.md](MAINTENANCE_GUIDE.md) - Test maintenance
- [Playwright Discord](https://discord.com/invite/playwright-807756831384403968)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/playwright)

## Next Steps

- Review [TEST_SUITE.md](TEST_SUITE.md) for test coverage details
- Read [MAINTENANCE_GUIDE.md](MAINTENANCE_GUIDE.md) for adding new tests
- Check [README.md](README.md) for authentication utilities
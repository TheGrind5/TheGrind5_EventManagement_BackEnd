/**
 * AI Generated Test Runner
 * Runs all Shopping Cart related tests
 */

const { execSync } = require('child_process');
const path = require('path');

console.log('🧪 AI Generated Test Runner for Shopping Cart');
console.log('==============================================\n');

// Test files to run
const testFiles = [
  'ShoppingCart.test.js',
  'ShoppingCartClass.test.js',
  'WishlistContext.test.js'
];

// Colors for console output
const colors = {
  green: '\x1b[32m',
  red: '\x1b[31m',
  yellow: '\x1b[33m',
  blue: '\x1b[34m',
  reset: '\x1b[0m',
  bold: '\x1b[1m'
};

function runTest(testFile) {
  try {
    console.log(`${colors.blue}Running ${testFile}...${colors.reset}`);
    
    const command = `npx jest src/__tests__/${testFile} --verbose --no-coverage`;
    const output = execSync(command, { 
      encoding: 'utf8',
      cwd: path.join(__dirname, '..', '..')
    });
    
    console.log(`${colors.green}✅ ${testFile} - PASSED${colors.reset}\n`);
    return { file: testFile, status: 'PASSED', output };
    
  } catch (error) {
    console.log(`${colors.red}❌ ${testFile} - FAILED${colors.reset}`);
    console.log(`${colors.red}Error: ${error.message}${colors.reset}\n`);
    return { file: testFile, status: 'FAILED', error: error.message };
  }
}

function runAllTests() {
  console.log(`${colors.bold}Starting test execution...${colors.reset}\n`);
  
  const results = [];
  const startTime = Date.now();
  
  // Run each test file
  testFiles.forEach(testFile => {
    const result = runTest(testFile);
    results.push(result);
  });
  
  const endTime = Date.now();
  const totalTime = (endTime - startTime) / 1000;
  
  // Summary
  console.log(`${colors.bold}Test Summary:${colors.reset}`);
  console.log('================');
  
  const passed = results.filter(r => r.status === 'PASSED').length;
  const failed = results.filter(r => r.status === 'FAILED').length;
  
  console.log(`${colors.green}✅ Passed: ${passed}${colors.reset}`);
  console.log(`${colors.red}❌ Failed: ${failed}${colors.reset}`);
  console.log(`⏱️  Total Time: ${totalTime.toFixed(2)}s`);
  
  if (failed > 0) {
    console.log(`\n${colors.red}Failed Tests:${colors.reset}`);
    results
      .filter(r => r.status === 'FAILED')
      .forEach(r => console.log(`  - ${r.file}: ${r.error}`));
  }
  
  console.log(`\n${colors.bold}Test execution completed!${colors.reset}`);
  
  return results;
}

// Export for use in other files
module.exports = {
  runAllTests,
  runTest,
  testFiles
};

// Run if called directly
if (require.main === module) {
  runAllTests();
}

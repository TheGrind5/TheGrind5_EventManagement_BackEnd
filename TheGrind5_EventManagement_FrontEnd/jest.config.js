module.exports = {
  // Test environment
  testEnvironment: 'jsdom',
  
  // Setup files
  setupFilesAfterEnv: ['<rootDir>/src/setupTests.js'],
  
  // Module name mapping for absolute imports
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/src/$1',
    '^@components/(.*)$': '<rootDir>/src/components/$1',
    '^@services/(.*)$': '<rootDir>/src/services/$1',
    '^@contexts/(.*)$': '<rootDir>/src/contexts/$1',
    '^@utils/(.*)$': '<rootDir>/src/utils/$1',
  },
  
  // Transform files
  transform: {
    '^.+\\.(js|jsx)$': 'babel-jest',
  },
  
  // Test file patterns
  testMatch: [
    '<rootDir>/src/**/__tests__/**/*.(js|jsx)',
    '<rootDir>/src/**/*.(test|spec).(js|jsx)',
  ],
  
  // Coverage configuration
  collectCoverageFrom: [
    'src/**/*.(js|jsx)',
    '!src/index.js',
    '!src/setupTests.js',
    '!src/**/*.stories.(js|jsx)',
  ],
  
  // Coverage thresholds
  coverageThreshold: {
    global: {
      branches: 70,
      functions: 70,
      lines: 70,
      statements: 70,
    },
  },
  
  // Module file extensions
  moduleFileExtensions: ['js', 'jsx', 'json'],
  
  // Clear mocks between tests
  clearMocks: true,
  
  // Restore mocks after each test
  restoreMocks: true,
  
  // Verbose output
  verbose: true,
};

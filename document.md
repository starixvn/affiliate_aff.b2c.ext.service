# B2C External Service API Development Documentation

## Overview

This document details the comprehensive development and enhancement of a B2C external service API with authorization and statistics functionality.

## Initial Development

### GetAffMediaChannelAsync Endpoint
- **Added GetAffMediaChannelAsync endpoint** with parameters:
  - FromDate
  - ToDate
  - PageIndex (default 1)
  - PageSize (default 1000)
- **Created AffMediaChannelCriteria model** following existing patterns with SpParam attributes
- **Created AffMediaChannel DTO** initially with multiple properties, later simplified to only ChannelCode and ChannelName
- **Added endpoint to StatisticsController** with proper logging and validation

## Statistics Service Architecture

### Controller Structure
- **Created StatisticsController** as a new controller for statistics-related endpoints
- **Moved GetAffMediaChannelAsync** from AffInfoController to StatisticsController
- **Created IStatisticsService and StatisticsService** following repository pattern

### Click Statistics Endpoint
- **Added GetClickStatisticAsync endpoint** with comprehensive click tracking data including 22+ properties:
  - ClickId, AffId, AffCode, AffUserName
  - Browser details, device info
  - Geographic information
  - Referral tracking
  - Campaign data
- **Created ClickStatisticCriteria and ClickStatistic DTO** with full database schema mapping

## Authorization System Overhaul

### Enhanced User Model
- **Enhanced User model** to include SupportedFunctions property

### Comprehensive Authorization Framework
- **FunctionAuthorizeAttribute** for function-based authorization
- **FunctionRequirement and FunctionAuthorizationHandler**
- **LicenseeRequirement and LicenseeAuthorizationHandler**
- **Function constants class** with AffInfo, Statistics, Exchange values

### Applied Authorization Policies
- **AffInfoController**: "AffInfo" policy + licensee validation
- **ExchangeController**: "Exchange" policy
- **StatisticsController**: "Statistics" policy + licensee validation

## User Configuration Updates

### Updated Configuration Files
- **Updated all user configuration files** (Development, LOCAL, UAT, PROD) to include SupportedFunctions

### Function Permission Assignment
- **B2C users**: AffInfo, Exchange
- **Statistics users (12B)**: Statistics only
- **EXT_AFF_API**: All functions (AffInfo, Statistics, Exchange)

## Validation and Error Handling

### Page Validation
- **Added PageSize validation**:
  - Maximum 1000 for ClickStatistic
  - Maximum 2000 for AffDetailInfo
- **Added PageIndex validation** with minimum value of 1

### Authorization Error Handling
- **Fixed "Page not found" issue** caused by 403 Forbidden being mishandled
- **Updated ErrorController** to properly handle 401, 403, 404 status codes
- **Fixed authentication scheme errors** by replacing Forbid() with StatusCode(403)

## Authorization Implementation Best Practices

### Migration to Policy-Based Authorization
- **Migrated from ActionFilter to Policy-based authorization** following ASP.NET Core best practices
- **Created AuthorizationExtensions** for clean DI registration

### Two-Tier Authorization System
- **Function-level**: Policy-based authorization in middleware
- **Resource-level**: Licensee authorization in controller actions
- **Fixed configuration conflicts** by unifying authorization setup in Startup.cs

## Testing and Documentation

### Unit Tests
- **Updated unit tests** for AffInfoController to handle new IAuthorizationService dependency and async methods

### API Documentation
- **Generated comprehensive API documentation** for StatisticsController endpoints following template format
- Included request/response schemas, validation rules, and examples

## Technical Improvements

### Code Quality
- **Removed redundant attributes** (deleted specific authorize attributes in favor of unified FunctionAuthorizeAttribute)
- **Fixed namespace issues** and import statements
- **Implemented proper HTTP status codes** (401 for authentication, 403 for authorization)
- **Added proper async/await patterns** throughout the codebase

## Final System Architecture

The final system provides:
- **Robust, enterprise-grade authorization framework**
- **Comprehensive statistics tracking capabilities**
- **Proper validation and error handling**
- **Clear API documentation**
- **Scalable service architecture**

## Key Files Modified/Created

### Controllers
- `AffInfoController.cs` - Enhanced with authorization service
- `StatisticsController.cs` - New controller for statistics endpoints
- `ExchangeController.cs` - Enhanced with policy-based authorization

### Models
- `AffMediaChannelCriteria.cs` - New criteria model
- `AffMediaChannel.cs` - New DTO model
- `ClickStatisticCriteria.cs` - New criteria model
- `ClickStatistic.cs` - New DTO model with 22+ properties

### Services
- `IStatisticsService.cs` - New service interface
- `StatisticsService.cs` - New service implementation

### Authorization
- `FunctionAuthorizeAttribute.cs` - New authorization attribute
- `FunctionRequirement.cs` - New authorization requirement
- `FunctionAuthorizationHandler.cs` - New authorization handler
- `LicenseeRequirement.cs` - New licensee requirement
- `LicenseeAuthorizationHandler.cs` - New licensee handler
- `AuthorizationExtensions.cs` - New DI extensions

### Configuration
- Updated all `Users.*.json` files with SupportedFunctions
- Enhanced `Startup.cs` with authorization configuration

## API Endpoints

### Statistics Controller
- `POST /api/StatisticsMediaChannel` - Get affiliate media channels
- `POST /api/ClickStatistic` - Get click statistics with comprehensive tracking data

### AffInfo Controller  
- `POST /api/AffInfo` - Get affiliate information
- `POST /api/AffDetailInfoList` - Get detailed affiliate information

### Exchange Controller
- `POST /api/ExchangeRate` - Get exchange rates

All endpoints include proper authorization, validation, and error handling. 
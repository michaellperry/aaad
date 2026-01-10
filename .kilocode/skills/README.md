# GloboTicket Skills Overview

This directory contains Kilo Code skills that provide specialized knowledge and workflows for the GloboTicket project. Skills are automatically loaded based on the current mode to enhance the AI agent's capabilities.

## Available Skills

### 🏗️ Architecture & Design (Generic Skills)

These skills are available in all modes:

#### clean-architecture
- **Purpose**: Clean Architecture principles and implementation patterns for .NET applications
- **Coverage**: Layer separation, dependency rules, entity patterns, repository patterns, use case patterns
- **Use When**: Implementing new features, refactoring existing code, or designing system architecture

#### multi-tenancy-patterns  
- **Purpose**: Multi-tenancy implementation patterns and tenant isolation strategies
- **Coverage**: Row-level security, tenant context resolution, data isolation, security considerations
- **Use When**: Working with tenant-aware entities, implementing new tenant features, or debugging tenant issues

### ⚙️ Code Mode Specific Skills

These skills are only available when using Code mode:

#### linq-query-patterns
- **Purpose**: LINQ query syntax patterns, separation of concerns, and async execution for data access
- **Coverage**: Query syntax conventions, repository patterns, async/await best practices
- **Use When**: Writing repository methods, implementing LINQ queries, or refactoring data access code

#### unit-testing
- **Purpose**: Comprehensive unit testing patterns for .NET applications using EF Core In-Memory Provider
- **Coverage**: AAA structure, TDD workflows, test data helpers, EF Core in-memory setup, multi-tenancy testing, mocking strategies
- **Use When**: Writing unit tests for services, repositories, or domain logic; implementing TDD workflows

#### spec-writing-database
- **Purpose**: Database schema specifications using Mermaid ER diagrams
- **Coverage**: Entity-relationship diagrams, table structures, multi-tenant entities
- **Use When**: Designing database schemas, creating ER diagrams, or specifying database constraints

#### spec-writing-openapi
- **Purpose**: Complete OpenAPI 3.0 specifications for API endpoints
- **Coverage**: RESTful conventions, request/response schemas, API documentation
- **Use When**: Creating new API endpoints, documenting API contracts, or defining API specifications

#### spec-writing-tests
- **Purpose**: Test scenarios using Given-When-Then format
- **Coverage**: Test requirements, acceptance criteria mapping, unit and integration test scenarios
- **Use When**: Writing technical specifications, defining test requirements, or ensuring test coverage

#### spec-writing-ui
- **Purpose**: User interface designs, React components, and interaction flows
- **Coverage**: Component-based architecture, props and state, user interaction flows
- **Use When**: Designing UI specifications, defining component structures, or documenting user interactions

#### user-story-acceptance-criteria
- **Purpose**: User stories with acceptance criteria following standard categories
- **Coverage**: Story structure, acceptance criteria patterns, completeness review
- **Use When**: Creating user stories, defining acceptance criteria, or reviewing story completeness

## How Skills Work

Skills are automatically discovered and loaded by Kilo Code when:

1. **VSCode starts** or when you **reload the window** (Cmd+Shift+P → "Developer: Reload Window")
2. **The current mode matches** the skill's scope:
   - Generic skills load in all modes
   - Mode-specific skills only load when that mode is active

## Skill Structure

Each skill follows the Agent Skills specification:

```
.kilocode/
├── skills/                     # Generic skills (all modes)
│   ├── clean-architecture/
│   │   └── SKILL.md
│   └── multi-tenancy-patterns/
│       └── SKILL.md
└── skills-code/               # Code mode specific skills
    ├── ef-core-patterns/
    │   └── SKILL.md
    ├── tdd-testing-patterns/
    │   └── SKILL.md
    └── api-rest-conventions/
        └── SKILL.md
```

## Integration with Custom Modes

These skills enhance your existing custom modes:

- **🟣 EF Entity Generator**: Enhanced by `ef-core-patterns` for better entity design
- **🟡 Implementation Validator**: Benefits from all architecture and pattern skills
- **🩷 Spec Writer**: Improved by `api-rest-conventions` for API specifications
- **🟢 TDD Implementation**: Enhanced by `tdd-testing-patterns` for better test implementation
- **🔴 TDD Test First**: Benefits from `tdd-testing-patterns` for comprehensive test strategies
- **🟠 User Story Writer**: Can leverage `clean-architecture` for better architectural context

## Best Practices

### When to Use Skills
- Reference skills when working on features that match their domain
- Skills provide deep, specialized knowledge that enhances the AI's understanding
- Each skill contains practical examples and code patterns specific to GloboTicket

### Skill Selection
- **Generic skills** provide architectural and cross-cutting concerns
- **Code mode skills** provide implementation-specific guidance
- Skills are complementary - use multiple skills when working on complex features

### Adding New Skills
To add new skills to this project:

1. Create a new directory in the appropriate location:
   - `.kilocode/skills/` for generic skills
   - `.kilocode/skills-{mode}/` for mode-specific skills

2. Create a `SKILL.md` file with YAML frontmatter:
   ```yaml
   ---
   name: your-skill-name
   description: Brief description of what this skill does
   ---
   ```

3. Add detailed instructions in Markdown format

4. Ensure the `name` field matches the directory name exactly

5. Reload VSCode to load the new skill

## Troubleshooting

### Skills Not Loading
- **Check VSCode output**: View → Output → Select "Kilo Code" for error messages
- **Verify structure**: Ensure `SKILL.md` is directly in the skill directory
- **Reload VSCode**: Use Cmd+Shift+P → "Developer: Reload Window"
- **Check naming**: The `name` field in frontmatter must match the directory name

### Common Issues
- **YAML syntax errors**: Ensure proper indentation and quoting
- **File location**: Skills must be in `.kilocode/skills/` or `.kilocode/skills-{mode}/`
- **Mode specificity**: Mode-specific skills only load in matching modes

## Resources

- [Agent Skills Specification](https://agentskills.io/)
- [Kilo Code Skills Documentation](https://kilo.ai/docs/features/skills)
- [Custom Modes Documentation](https://kilo.ai/docs/features/custom-modes)

These skills provide specialized knowledge that enhances your development workflow while maintaining the speed and efficiency of Kilo Code's AI-powered assistance.
# OpenAPI Specification

Provide complete OpenAPI 3.0 definitions for ALL new or modified endpoints:

```yaml
openapi: 3.0.0
info:
  title: GloboTicket API - [Feature Name]
  version: 1.0.0

paths:
  /api/resource:
    post:
      summary: Create new resource
      tags:
        - Resource
      security:
        - cookieAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/CreateResourceRequest'
      responses:
        '201':
          description: Resource created successfully
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/ResourceResponse'
        '400':
          description: Invalid request
        '401':
          description: Unauthorized
        '429':
          description: Rate limit exceeded

components:
  schemas:
    CreateResourceRequest:
      type: object
      required:
        - name
      properties:
        name:
          type: string
          maxLength: 100
          example: "Resource Name"
    
    ResourceResponse:
      type: object
      properties:
        id:
          type: integer
          format: int32
        entityGuid:
          type: string
          format: uuid
        name:
          type: string
        createdAt:
          type: string
          format: date-time
  
  securitySchemes:
    cookieAuth:
      type: apiKey
      in: cookie
      name: .GloboTicket.Auth
```

## API Design Principles

- Use RESTful conventions (GET, POST, PUT, DELETE)
- Return appropriate status codes (201 for created, 204 for no content)
- Follow existing endpoint patterns in codebase
- Require authorization by default
- Specify all validation rules and constraints
- Document error responses (400, 401, 403, 404, 429, 500)

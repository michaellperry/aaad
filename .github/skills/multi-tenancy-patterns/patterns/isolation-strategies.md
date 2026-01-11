# Isolation Strategies

Compare approaches: database per tenant, schema per tenant, row level security, discriminator column.

## Database per Tenant
**Pros:** Maximum data isolation, simple backup/restore per tenant  
**Cons:** High operational overhead, resource intensive, complex migrations  
**Use Case:** Large enterprise tenants with strict compliance requirements

## Schema per Tenant
**Pros:** Good isolation, manageable operational overhead  
**Cons:** Limited scalability, complex connection management  
**Use Case:** Medium-sized tenants, moderate tenant count

## Row Level Security (RLS) - Recommended
**Pros:** Shared infrastructure, cost-effective, scalable  
**Cons:** Requires careful security implementation, potential for data leaks  
**Use Case:** GloboTicket's approach - many small-medium tenants

## Discriminator Column
**Pros:** Simple implementation, shared queries  
**Cons:** Poor isolation, complex queries, scalability issues  
**Use Case:** Not recommended for production use

GloboTicket uses Row Level Security with shared database for cost-effectiveness and scalability.

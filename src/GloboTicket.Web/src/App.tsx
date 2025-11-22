function App() {
  return (
    <div style={{ padding: '2rem', maxWidth: '800px', margin: '0 auto' }}>
      <h1>GloboTicket - Multi-Tenant Event Ticketing</h1>
      <p style={{ fontSize: '1.1rem', marginTop: '1rem' }}>
        Frontend is configured. API integration ready.
      </p>
      <div style={{ 
        marginTop: '2rem', 
        padding: '1rem', 
        backgroundColor: '#f5f5f5', 
        borderRadius: '4px' 
      }}>
        <h2 style={{ fontSize: '1.2rem', marginBottom: '0.5rem' }}>Architecture Setup Complete</h2>
        <ul style={{ lineHeight: '1.8' }}>
          <li>✓ React 19.2 with TypeScript</li>
          <li>✓ Vite development server</li>
          <li>✓ Tanstack Query for server state management</li>
          <li>✓ API client with cookie-based authentication</li>
          <li>✓ Proxy configuration for API routes</li>
        </ul>
      </div>
    </div>
  )
}

export default App

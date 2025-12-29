import React from 'react'
import { NavProvider } from './NavContext'
import { AuthProvider } from './AuthContext'
import { ValideFormProvider } from './ValideFormContext'

function Index({ children }) {
  return (
    <NavProvider>
      <AuthProvider>
        <ValideFormProvider>
          {children}
        </ValideFormProvider>
      </AuthProvider>
    </NavProvider>
  )
}

export default Index
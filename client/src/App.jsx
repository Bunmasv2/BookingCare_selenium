import { ToastContainer } from 'react-toastify'
import 'react-toastify/dist/ReactToastify.css'
import AppRoute from './Route/AppRoute'
import './Style/App.css'

function App() {
  return (
    <div className="App">
      <ToastContainer />
      <AppRoute />
    </div>
  )
}

export default App
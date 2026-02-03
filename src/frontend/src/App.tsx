import './App.css'
import './index.css'
import DashBoard from './components/DashBoard'
function App() {

  // Create WebSocket connection.
  const socket = new WebSocket("ws://localhost:8080");

  // Listen for messages
  socket.addEventListener("message", (event) => {
    console.log("Message from server ", event.data);
  });



  return (
    <>
      <DashBoard />

    </>
  )
}

export default App

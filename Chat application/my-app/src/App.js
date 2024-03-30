
import './App.css';

import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from './components/Login'; 
import { AuthProvider } from './contexts/AuthContext';
import Chats from './components/Chat';

function App() {
  return (
    <div style={{ fontFamily: 'Avenir' }}>
      <Router>
        <AuthProvider>
        <Routes>
        <Route path="/chats" element={<Chats/>} />
          <Route path="/" element={<Login />} /> 
        </Routes>
        </AuthProvider>
      </Router>
    </div>
  );
}

export default App;




// import React from 'react';
// import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
// import Login from './Login'; 

// function App() {
//   return (
//     <div style={{ fontFamily: 'Avenir' }}>
//       <Router>
//         <Routes>
//           <Route path="/" element={<Login />} /> 
//         </Routes>
//       </Router>
//     </div>
//   );
// }

// export default App;
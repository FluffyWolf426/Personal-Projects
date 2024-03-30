
import './App.css';

import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from './components/Login'; 

function App() {
  return (
    <div style={{ fontFamily: 'Avenir' }}>
      <Router>
        <Routes>
          <Route path="/" element={<Login />} /> 
        </Routes>
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
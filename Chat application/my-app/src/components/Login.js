import React from "react";
import { GoogleOutlined, FacebookOutlined } from "@ant-design/icons";

import { auth } from "./firebase";

import firebase from 'firebase/compat/app'; 
import 'firebase/compat/auth';


const Login = () => {
  return (
    <div id="login-page">
      <div id="login-card">
        <h2>Welcome to CrazyChat!</h2>
        <div className="login-button google"
        onClick={() => auth.signInWithRedirect(new firebase.auth.GoogleAuthProvider())}>
          
          <GoogleOutlined /> Sign In with Google
        </div>
        <br/> <br/>
        <div className="login-button facebook"
          onClick={() => auth.signInWithRedirect(new firebase.auth.FacebookAuthProvider())}>
          <FacebookOutlined /> Sign In with Facebook
        </div>
      </div>
    </div>
  );
};

export default Login


// import React from "react";
// import { GoogleOutlined, FacebookOutlined } from "@ant-design/icons";
// import { signInWithRedirect } from "firebase/auth";
// import { auth } from "./firebase";
// import firebase from 'firebase/compat/app'; 
// import 'firebase/compat/auth';

// const Login = () => {
//   const handleGoogleSignIn = () => {
//     signInWithRedirect(auth, new firebase.auth.GoogleAuthProvider());
//   };

//   const handleFacebookSignIn = () => {
//     signInWithRedirect(auth, new firebase.auth.FacebookAuthProvider());
//   };

//   return (
//     <div id="login-page">
//       <div id="login-card">
//         <h2>Welcome to CrazyChat!</h2>
//         <div className="login-button google" onClick={handleGoogleSignIn}>
//           <GoogleOutlined /> Sign In with Google
//         </div>
//         <br/> <br/>
//         <div className="login-button facebook" onClick={handleFacebookSignIn}>
//           <FacebookOutlined /> Sign In with Facebook
//         </div>
//       </div>
//     </div>
//   );
// };

// export default Login;

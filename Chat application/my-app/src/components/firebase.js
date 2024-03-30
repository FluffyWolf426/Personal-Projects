import firebase from 'firebase/compat/app';
import 'firebase/compat/auth'; 

export const auth = firebase.initializeApp({
    apiKey: "AIzaSyC98q3mPg3vhwlEq5R97dH_krn2Ok21vmA",
    authDomain: "crazychat-15c7d.firebaseapp.com",
    projectId: "crazychat-15c7d",
    storageBucket: "crazychat-15c7d.appspot.com",
    messagingSenderId: "913890099026",
    appId: "1:913890099026:web:48516b4ce529513bf8848b"
  }).auth()

  

//   import firebase from 'firebase/compat/app';
// import 'firebase/compat/auth'; 

// // Your Firebase configuration details
// const firebaseConfig = {
//   apiKey: "AIzaSyC98q3mPg3vhwlEq5R97dH_krn2Ok21vmA",
//   authDomain: "crazychat-15c7d.firebaseapp.com",
//   projectId: "crazychat-15c7d",
//   storageBucket: "crazychat-15c7d.appspot.com",
//   messagingSenderId: "913890099026",
//   appId: "1:913890099026:web:48516b4ce529513bf8848b"
// };

// // Initialize Firebase app
// const app = firebase.initializeApp(firebaseConfig);

// // Get the Auth service instance
// const auth = app.auth(); 

// export { auth }; // Export only the 'auth' object














//   import firebase from "firebase/app";
// import "firebase/auth";

// const firebaseConfig = {
//   apiKey: "YOUR_API_KEY",
//   authDomain: "YOUR_AUTH_DOMAIN",
//   projectId: "YOUR_PROJECT_ID",
//   storageBucket: "YOUR_STORAGE_BUCKET",
//   messagingSenderId: "YOUR_MESSAGING_SENDER_ID",
//   appId: "YOUR_APP_ID"
// };

// // Initialize Firebase
// const app = firebase.initializeApp(firebaseConfig);
// export const auth = app.auth();
// export default app;
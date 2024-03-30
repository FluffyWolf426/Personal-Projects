import React from "react";
import { useNavigate } from "react-router-dom";
import { ChatEngine } from "react-chat-engine";
import { auth } from "./firebase";


const Chats = () => {
  
    const navigate = useNavigate()

    const handleLogout = async () =>{
        await auth.signOut()
        navigate('/chats')
    }
  return (
    <div className="chats-page">
      <div className="nav-bar">
        <div className="logo-tab">
            CrazyChat
        </div>
        <div onClick = {handleLogout}
             className="logout-tab">
            Logout
        </div>
      </div>
      <ChatEngine height="clac(100vh - 66px)"
                  projectId="3525e81e-df15-4be4-84da-be62b9fdfa48"
                  userName="."
                  userSecret="."  //Dot means that we actually need a real user name and a Secret to be able to login

        />
    </div>
  );
};

export default Chats;

import { useContext, useState, useEffect} from "react";
import React from "react";
import { useNavigate } from "react-router-dom";
import { auth } from "../components/firebase";

const AuthContext = React.createContext()

export const useAuth = () => useContext(AuthContext)

export const AuthProvider = ({children}) => {
    const [loading, setLoding] = useState(true)
    const [user, setUser] = useState(null) 
    const navigate = useNavigate()

    useEffect(() => {
 auth.onAuthStateChanged((user) => {
 setUser(user)
 setLoding(false)
 if(user) navigate('/chats')
//  history.push('/chats') old version of the hook
 })
    }, [user, navigate])

    const value = {user}

    return (
        <AuthContext.Provider value={value}>
        {!loading && children}
        </AuthContext.Provider>
    )
}
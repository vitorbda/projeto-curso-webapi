import React from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Login from './pages/Login';
import Alunos from './pages/Alunos';

export default function RoutesApp() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" exact Component={Login} />
                <Route path='/alunos' Component={Alunos} />
            </Routes>
        </BrowserRouter>
    )
}

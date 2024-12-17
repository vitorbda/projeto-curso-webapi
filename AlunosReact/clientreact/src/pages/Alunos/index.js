import React, { useEffect, useState } from 'react';
import {Link, useNavigate} from 'react-router-dom';
import './style.css';
import logoCadastro from '../../assets/cadastro.png';
import { FiEdit, FiUserX, FiXCircle } from 'react-icons/fi';
import api from '../../services/api';

export default function Alunos() {

    const [nome, setNome] = useState('');
    const [alunos, setAlunos] = useState([]);

    const email = localStorage.getItem('email');
    const token = localStorage.getItem('token');

    const navigate = useNavigate();

    const authorization = {
        headers: {
            Authorization: `bearer ${token}`
        }
    }

    async function logout() {
        try {
            localStorage.clear();
            localStorage.setItem('token', '');
            authorization.headers = '';

            navigate('/');
        } 
        catch (error) {
            alert(error);
        }
    }

    useEffect(() => {
        api.get('api/alunos', authorization)
        .then(
            response => {setAlunos(response.data)})
    },[])

    return (
        <div className='aluno-container'>
            <header>
                <img src={logoCadastro} alt='Cadastro' />

                <span>
                    Bem vindo, <strong>{email}!</strong>
                </span>
                <Link className='button' to='/aluno/novo/0'>Novo Aluno</Link>
                <button type='button' onClick={logout}>
                    <FiXCircle size={35} color='#17202a' />
                </button>                
            </header>

            <form>
                <input type='text' placeholder='Nome' />
                <button type='button' className='button'>
                    Filtrar aluno por nome (parcial)
                </button>
            </form>
            <h1>Relação de Alunos</h1>

            <ul>
                {alunos.map(aluno => (
                    <li key={aluno.id}>
                        <b>Nome: </b> {aluno.nome} <br/><br/>
                        <b>Email: </b> {aluno.email} <br/><br/>
                        <b>Idade: </b> {aluno.idade} <br/><br/>

                        <button type='button'>
                            <FiEdit size={25} color='#17202a' />
                        </button>
                        <button type='button'>
                            <FiUserX size={25} color='#17202a' />
                        </button>
                    </li>
                ))}
                
            </ul>

        </div>
    )
}
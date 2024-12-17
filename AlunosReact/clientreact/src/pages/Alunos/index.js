import React, { useEffect, useState } from 'react';
import {Link, useNavigate} from 'react-router-dom';
import './style.css';
import logoCadastro from '../../assets/cadastro.png';
import { FiEdit, FiUserX, FiXCircle } from 'react-icons/fi';
import api from '../../services/api';

export default function Alunos() {

    const [searchInput, setSearchInput] = useState('');
    const [filtro, setFiltro] = useState([]);

    const [alunos, setAlunos] = useState([]);

    const email = localStorage.getItem('email');
    const token = localStorage.getItem('token');

    const navigate = useNavigate();

    const authorization = {
        headers: {
            Authorization: `bearer ${token}`
        }
    }

    const searchAlunos = () => {        
        if (searchInput !== '') {
            const dadosFiltrados = alunos.filter((item) => {
                return Object.values(item)
                .join('').toLowerCase()
                .includes(searchInput.toLowerCase())
            });

            setFiltro(dadosFiltrados);
            return;
        }

        setFiltro(alunos);
    }

    useEffect(() => {
        searchAlunos();
    }, [searchInput]);

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

    async function editAluno(id) {
        try {
            navigate(`/aluno/novo/${id}`);
        } 
        catch (error) {
            
        }
    }

    async function deleteAluno(id) {
        if (window.confirm(`Deseja deletar o aluno ${alunos.filter(a => a.id == id)[0].nome}?`)){
            await api.delete(`/api/alunos/${id}`, authorization);
            
            const alunosAtualizados = alunos.filter(a => a.id !== id);
            setAlunos(alunosAtualizados);
            setFiltro(alunosAtualizados);
        }
    }

    useEffect(() => {
        api.get('api/alunos', authorization)
        .then(
            response => {
                setAlunos(response.data);
                setFiltro(response.data);
            })
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
                <input type='text' placeholder='Nome' 
                    value={searchInput}
                    onChange={e => setSearchInput(e.target.value)}
                />
            </form>
            <h1>Relação de Alunos</h1>

            <ul>
                {filtro.map(aluno => (
                    <li key={aluno.id}>
                        <b>Nome: </b> {aluno.nome} <br/><br/>
                        <b>Email: </b> {aluno.email} <br/><br/>
                        <b>Idade: </b> {aluno.idade} <br/><br/>

                        <button type='button' onClick={() => editAluno(aluno.id)}>
                            <FiEdit size={25} color='#17202a' />
                        </button>
                        <button type='button' onClick={() => deleteAluno(aluno.id)}>
                            <FiUserX size={25} color='#17202a' />
                        </button>
                    </li>
                ))}
                
            </ul>

        </div>
    )
}
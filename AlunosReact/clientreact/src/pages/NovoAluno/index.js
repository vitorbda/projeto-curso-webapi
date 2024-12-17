import React, { useEffect, useState } from 'react';
import './style.css';
import { FiCornerDownLeft, FiUserPlus } from 'react-icons/fi';
import { Link, useNavigate, useParams } from 'react-router-dom';
import api from '../../services/api';


export default function NovoAluno() {

    const {alunoId} = useParams();

    const [nome, setNome] = useState('');
    const [email, setEmail] = useState('');
    const [idade, setIdade] = useState('');

    const token = localStorage.getItem('token');

    const navigate = useNavigate();

    const authorization = {
        headers: {
            Authorization: `bearer ${token}`
        }
    }

    const textoHeader = alunoId === '0' ? 'Incluir novo aluno' : 'Atualizar aluno';
    const textoButton = alunoId === '0' ? 'Incluir' : 'Atualizar';

    async function saveOrUpdate(event) {
        event.preventDefault();

        const aluno = {
            id: alunoId,
            nome: nome,
            email: email,
            idade: idade
        };

        if (alunoId === '0') 
            await api.post('api/alunos', aluno, authorization);        
        else 
            await api.put('/api/alunos/' + alunoId, aluno, authorization)
        
        navigate('/alunos');
    }

    async function loadAluno() {
        try {
            const response = await api.get('/api/alunos/' + alunoId, authorization);

            setNome(response.data.nome);
            setEmail(response.data.email);
            setIdade(response.data.idade);
        } 
        catch (error) {
            
        }
    }

    useEffect(() => {
        if (alunoId === '0')
            return;

        loadAluno();
    }, [])

    return (
        <div className='novo-aluno-container'>
            <div className='content'>                

                <section className='form' >
                    <FiUserPlus size={105} color='#17202a' />
                    <h1>{textoHeader}</h1>
                    <Link className='back-link' to='/alunos'>
                        <FiCornerDownLeft size={25} color='#17202a' />
                        Retornar
                    </Link>
                    
                </section>

                <form onSubmit={saveOrUpdate}>
                    <input placeholder='Nome' onChange={e => setNome(e.target.value)} value={nome}/>
                    <input placeholder='Email' onChange={e => setEmail(e.target.value)} value={email}/>
                    <input placeholder='Idade' onChange={e => setIdade(e.target.value)} value={idade}/>
                    <button className='button' type='submit'> {textoButton} </button>
                </form>
            </div>
        </div>
    )
}

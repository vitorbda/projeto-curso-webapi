import React from 'react';
import './style.css';
import { FiCornerDownLeft, FiUserPlus } from 'react-icons/fi';
import { Link, useParams } from 'react-router-dom';


export default function NovoAluno() {

    const {alunoId} = useParams();

    const textoHeader = alunoId === '0' ? 'Incluir novo aluno' : 'Atualizar aluno';
    const textoButton = alunoId === '0' ? 'Incluir' : 'Atualizar';

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

                <form>
                    <input placeholder='Nome' />
                    <input placeholder='Email' />
                    <input placeholder='Idade' />
                    <button className='button' type='submit'> {textoButton} </button>
                </form>
            </div>
        </div>
    )
}

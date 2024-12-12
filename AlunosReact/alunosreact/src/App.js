import './App.css';

import 'bootstrap/dist/css/bootstrap.min.css';
import axios from 'axios';
import { Modal, ModalBody, ModalFooter, ModalHeader } from 'reactstrap';
import logoCadastro from './assets/cadastro.png';
import { useEffect, useState } from 'react';

function App() {

  const baseUrl = 'https://localhost:7054/api/Alunos';

  const[data, setData] = useState([]);

  const [alunoSelecionado, setAlunoSelecionado] = useState({id: '', nome: '', email: '', idade: ''});

  const [modalIncluir, setModalIncluir] = useState(false);

  const abrirFecharModal = () => {
    setModalIncluir(!modalIncluir);
  };

  const handleChange = e => {
    const { name, value } = e.target;
    setAlunoSelecionado({
      ...alunoSelecionado, [name]: value
    });
    console.log(alunoSelecionado);
  };

  const pedidoGet = async() => {
    await axios.get(baseUrl)
    .then(response => {
      setData(response.data);
    })
    .catch(error => {
      console.log(error);
    })
  };

  const pedidoPost = async() => {
    delete alunoSelecionado.id;
    alunoSelecionado.idade = parseInt(alunoSelecionado.idade);

    await axios.post(baseUrl, alunoSelecionado)
    .then(response => {
      setData(data.concat(response.data));
      abrirFecharModal();
    })
    .catch(error => {
      console.log(error);
    })
  };

  useEffect(() => {
    pedidoGet();
  },[])

  return (
    <div className="aluno-container">
      <br />
      <h3>Cadastro de alunos</h3>

      <header>
        <img src={logoCadastro} alt='Cadastro' />
        <button className='btn btn-success' onClick={abrirFecharModal}>Incluir novo aluno</button>
      </header>

      <table className='table table-bordered'>
        <thead>
          <tr>
            <th>Id</th>
            <th>Nome</th>
            <th>Email</th>
            <th>Idade</th>
            <th>Operação</th>
          </tr>
        </thead>

        <tbody>

          {data.map(aluno => (
            <tr key={aluno.id}>
              <td>{aluno.id}</td>
              <td>{aluno.nome}</td>
              <td>{aluno.email}</td>
              <td>{aluno.idade}</td>
              <td>
                <button className='btn btn-primary'>Editar</button> {"  "}
                <button className='btn btn-danger'>Excluir</button>
              </td>
            </tr>
          ))}

        </tbody>

      </table>


      <Modal isOpen={modalIncluir}>

        <ModalHeader>Incluir Alunos</ModalHeader>

        <ModalBody>
          <div className='form-group'>
            <label>Nome: </label>
            <br />
            <input type='text' name='nome' className='form-control' onChange={handleChange} />
            <br />
            <label>Email: </label>
            <br />
            <input type='text' name='email' className='form-control' onChange={handleChange} />
            <br />
            <label>Idade: </label>
            <br />
            <input type='number' name='idade' className='form-control' onChange={handleChange} />
          </div>
        </ModalBody>
        
        <ModalFooter>
          <button className='btn btn-primary' onClick={pedidoPost}>Incluir</button>{"   "}
          <button className='btn btn-danger' onClick={abrirFecharModal}>Cancelar</button>
        </ModalFooter>

      </Modal>



    </div>
  );
}

export default App;

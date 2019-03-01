import React from 'react';
import { render } from 'react-dom';

const App = () => (
    <>
        <h1>React in ASP.NET MVC!</h1>
        <div>Hello React World</div>
    </>
);

render(<App />, document.getElementById('app'));

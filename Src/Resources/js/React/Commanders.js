import React from 'react';
import { render } from 'react-dom';

const Commanders = () => (
    <div>
        <h1>Commaners Management!</h1>
        <div>Leadership Read/Write, Senior FCs read only</div>
    </div>
);

if (document.getElementById('commanders'))
    render(<Commanders />, document.getElementById('commanders'));
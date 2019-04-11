import React, { Component } from 'react';
import { render } from 'react-dom';

export class TqClock extends Component {

    constructor(props) {
        super(props);

        this.state = {
            time: new Date()
        }
    }

    componentDidMount() {
        this.intervalID = setInterval(
            () => this.tick(),
            1000
        );
    }

    componentWillUnmount() {
        clearInterval(this.intervalID);
    }

    tick() {
        this.setState({
            time: new Date()
        });
    }

    formatDate(n) { return (n < 10) ? "0" + n : n; }

    render() {
        return (
            <div className="clock _noselector hidden-md-down">
                <div className="time">
                    {this.formatDate(this.state.time.getUTCHours())} :
                    {this.formatDate(this.state.time.getUTCMinutes())} :
                    {this.formatDate(this.state.time.getUTCSeconds())}
                </div>
            </div>
        );
    }
}

if (document.getElementById('tqClock'))
    render(<TqClock />, document.getElementById('tqClock'));
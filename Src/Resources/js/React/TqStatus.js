import React, { Component } from 'react';
import { render } from 'react-dom';

export class TqStatus extends Component {

    constructor(props) {
        super(props);
        this.state = {
            tq_status: 0
        }
    }

    componentDidMount() {
        // Initial TQ status
        this.tick();

        // Poll TQ status every 60 seconds.
        this.intervalID = setInterval(
            () => this.tick(),
            60 * 1000
        );
    }

    componentWillUnmount() {
        clearInterval(this.intervalID);
    }

    tick() {
        $.getJSON('https://esi.evetech.net/latest/status/?datasource=tranquility', (data) => {
            this.setState({
                tq_status: (!!data.players) ? data.players : 0
            });

            return;
        }).fail(() => {
            this.setState({ tq_status: 0 });
        });
    }

    render() {
        let status = {};
        if (!!this.state.tq_status && this.state.tq_status > 0) {
            status.val = this.state.tq_status.toLocaleString();
            status.class = "text-success";
        } else {
            status.val = "Offline";
            status.class = "text-danger";
        }

        return (
            <div className="clock _noselector hidden-md-down d-block">
                <div className={`time ${status.class}`}>
                    TQ: {status.val}
                </div>
            </div>
        );
    }
}

if (document.getElementById('tqStatus'))
    render(<TqStatus />, document.getElementById('tqStatus'));
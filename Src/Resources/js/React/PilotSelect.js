import React, { Component } from 'react';
import { render } from 'react-dom';
import ReactTooltip from 'react-tooltip'

const baseUri = '/pilot-select'

export default class PilotSelect extends Component {

    constructor(props) {
        super(props);

        this.state = {
            pilots: null
        }
    }

    componentDidMount() {
        this.getData();
    }

    getData() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/pilots`,
        }).done((pilots) => {
            this.setState({ pilots: pilots });
        }).fail((err) => {
            console.error(`[React/PilotSelect] @getData - Error retrieving account information`, err.responseText);
        })
    }

    getPilots() {
        return this.state.pilots;
    }

    render() {
        let loginCards;
        if (!!this.getPilots()) {
            loginCards = this.getPilots().map((pilot, index) => {
                return <LoginCard pilot={pilot} key={index} />
            });
        }

        return (
            <div className="content__inner">
                <div className="row">
                    <div class="col-12 pb-5">
                        <h1 className="text-center">Welcome to Imperium Incursions</h1>
                        <h3 className="text-center">Please select your main pilot to continue.</h3>
                    </div>

                    {loginCards}

                    <AddAccountCard />
                </div>
            </div>
        );
    }
}

export class AddAccountCard extends Component {
    render() {
        return (
            <div className="col-lg-3 col-md-6 col-sm-12">
                <div className="card login-card">
                    <img className="login-avatar" src={`https://image.eveonline.com/Character/0_256.jpg`} alt="No avatar found." />
                    <div className="login-name mb-4">Need another pilot?</div>

                    <a className="btn btn-primary d-block mx-auto my-4" href="/auth/eve">Login with Eve SSO</a>
                </div>
            </div>
        )
    }
}

export class LoginCard extends Component {

    getId() {
        return this.props.pilot.id || 0;
    }

    getName() {
        return this.props.pilot.name || ""
    }

    isEsiVaild() {
        return this.props.pilot && this.props.pilot.esiValid;
    }

    setMainPilot(id) {
        $.ajax({
            type: `post`,
            url: `${baseUri}/pilots/${id}`,
            statusCode: {
                200: () => {
                    location.href = '/';
                }
            }
        }).fail((err) => {
            console.error(`[React/PilotSelect] @setMainPilot - Error setting your main pilot`, err.responseText);
        });
    }

    render() {

        let esi = {}
        if (this.isEsiVaild()) {
            esi.label = <p className="login-esi text-success">ESI Valid</p>;
            esi.button = <button className="btn btn-success d-block mx-auto mb-4" onClick={this.setMainPilot.bind(this, this.getId())}>Proceed</button>;
        } else {
            esi.label = <p className="login-esi text-danger" data-multiline="true" data-tip="We require a valid ESI token before you can use this pilot.<br/>Please update your pilot ESI to continue.">ESI Invalid</p>;
            esi.button = <a className="btn btn-danger d-block mx-auto mb-4" href="/auth/eve">Update ESI</a>
        }

        return (
            <div className="col-lg-3 col-md-4 col-sm-6">
                <div className="card login-card">
                    <img className="login-avatar" src={`https://image.eveonline.com/Character/${this.getId()}_256.jpg`} alt={this.getName() + "\'s avatar."} />
                    <div className="login-name">{this.getName()}</div>

                    {esi.label}
                    {esi.button}

                    <ReactTooltip />
                </div>
            </div>
        )
    }
}

if (document.getElementById('pilotSelect'))
    render(<PilotSelect />, document.getElementById('pilotSelect'));

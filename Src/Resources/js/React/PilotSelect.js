import React, { Component } from 'react';
import { render } from 'react-dom';

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
        //Ajax call to API to get data
        $.ajax({
            type: 'get',
            url: '/pilot-select/pilots',
            //headers: { 'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content') }
        }).done((pilots) => {
            this.setState({ pilots: pilots, ready: true });
        }).fail((err) => {
            console.error(`React/PilotSelect {PilotSelect@getData} - Error getting your main pilots`, err);
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
            <div className="row">
                <div class="col-12 pb-5">
                    <h1 className="text-center">Welcome to Imperium Incursions</h1>
                    <h3 className="text-center">Please select your main pilot to continue.</h3>
                </div>

                {loginCards}

                {/* Special login card here */}
                <div className="col-lg-3 col-md-6 col-sm-12">
                    <div className="card login-card">
                        <img className="login-avatar" src={`https://image.eveonline.com/Character/0_256.jpg`} alt={"No avatar found."} />
                        <div className="login-name mb-4">Need another pilot?</div>

                        <a className="btn btn-primary d-block mx-auto my-4" href="/auth/eve">Login with Eve SSO</a>
                    </div>
                </div>
            </div>
        );
    }
}


export class LoginCard extends Component {

    getPilotId() {
        return (!!this.props.pilot && !!this.props.pilot.id) ? this.props.pilot.id : 0
    }

    getPilotName() {
        return (!!this.props.pilot && !!this.props.pilot.name) ? this.props.pilot.name : ""
    }

    isEsiVaild() {
        if (!!this.props.pilot && this.props.pilot.esiValid)
            return true;

        return false;
    }

    setMainPilot(character_id) {
        $.ajax({
            type: `post`,
            url: `/pilot-select/pilots/${character_id}`,
            statusCode: {
                200: () => {
                    location.href = '/';
                }
            }
        }).done((data) => {
            console.log("S")
        }).fail((err) => {
            console.error(`React/PilotSelect {LoginCard@setMainPilot} - Error setting your main pilot`, err);
        });
    }

    render() {

        let esi = {}
        if (this.isEsiVaild()) {
            esi.label = <p className="login-esi text-success">ESI Valid</p>;
            esi.button = <button className="btn btn-success d-block mx-auto mb-4" onClick={this.setMainPilot.bind(this, this.getPilotId())}>Proceed</button>;
        } else {
            esi.label = <p className="login-esi text-danger" data-toggle="tooltip" title="We require a valid ESI token before you can use this pilot. Please update your pilot ESI to continue.">ESI Invalid</p>;
            esi.button = <a className="btn btn-danger d-block mx-auto mb-4" href="/auth/eve">Update ESI</a>
        }

        return (
            <div className="col-lg-3 col-md-4 col-sm-6">
                <div className="card login-card">
                    <img className="login-avatar" src={`https://image.eveonline.com/Character/${this.getPilotId()}_256.jpg`} alt={this.getPilotName() + "\'s avatar."} />
                    <div className="login-name">{this.getPilotName()}</div>

                    {esi.label}
                    {esi.button}
                </div>
            </div>
        )
    }
}

if (document.getElementById('pilotSelect'))
    render(<PilotSelect />, document.getElementById('pilotSelect'));

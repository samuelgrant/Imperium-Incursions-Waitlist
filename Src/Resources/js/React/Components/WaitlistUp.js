import React, { Component } from 'react';
import Card from './Card';

export default class WaitlistUp extends Component {
    constructor(props) {
        super(props);

        this.state = {
            selectedPilot: null,
            selectRoles: [],
            selectedFits: []
        }
    }

    getPilots() {
        return (this.props.options && this.props.options.account) ? this.props.options.account.pilots : null;
    }

    getPrefPilot() {
        return (this.props.options) ? this.props.options.prefPilot : null;
    }

    getRoles() {
        return (this.props.options) ? this.props.options.roles : null;
    }

    getShips() {
        return (this.props.options) ? this.props.options.avaliableFits : null;
    }

    updateSelectedPilot(pilot_id) {
        this.setState({ selectedPilot: pilot_id });
    }

    render() {
        return (
            <div>
                <Card heading="Join the Waitlist">
                    <div className="row pb-4">
                        <div className="col-md-4 col-sm-12">
                            <WaitlistWith pilots={this.getPilots()} prefPilot={this.getPrefPilot()} selectedPilot={this.updateSelectedPilot.bind(this)} />
                        </div>

                        <div className="col-md-8 col-sm-12">
                            <SelectRoles roles={this.getRoles()} />
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-12">
                            <SelectShips fits={this.getShips()} />
                            <div class="clearfix"></div>
                            <button className="btn btn-success">Join the Waitlist <i className="fas fa-user-plus"></i></button>
                        </div>
                    </div>
                </Card>
            </div>
        )
    }
}

class SelectShips extends Component {    
    render() {//On change lets pass a list of roles up to the next component where we should set state.... use role ids: 1,2,5
        let fits;
        if (this.props.fits) {
            fits = this.props.fits.map((fit) => {
                return (
                    <div className="col-3">
                        <label className="custom-control custom-checkbox">
                            <input type="checkbox" className="custom-control-input" name={`fit:${fit.id}`} />
                            <span className="custom-control-indicator"></span>
                            <span className="custom-control-description"><img src={`https://image.eveonline.com/Render/${fit.shipTypeId}_32.png`} /> {fit.description}</span>
                        </label>
                    </div>
                )
            });
        }
        return (
            <div className="row  pb-4">
                <div className="col-12">
                    <h6>What ships do you wish to fly?</h6>
                </div>
                {fits}
            </div>
        )
    }
}

class SelectRoles extends Component {
    render() {//On change lets pass a list of roles up to the next component where we should set state.... use role ids: 1,2,5
        let roles;
        if (this.props.roles) {
            roles = this.props.roles.map((role) => {
                return (
                    <label className="custom-control custom-checkbox">
                        <input type="checkbox" className="custom-control-input" name={`role:${role.id}`}/>
                        <span className="custom-control-indicator"></span>
                        <span className="custom-control-description">{role.name}</span>
                    </label>
                );
            });
        }

        return (
            <div>
                <h6>What roles can you help us with?</h6>
                {roles}
            </div>
        )
    }
}

class WaitlistWith extends Component {
    updateSelectedPilot(e){
        this.props.selectedPilot(e.target.value);
    }

    render() {//On change of select let's pass the value up to the next component where we should set the state
        let pilotOptions;
        if (this.props.pilots != null) {
            pilotOptions = this.props.pilots.map((pilot) => {
                let selected = false;
                if (pilot.characterID == this.props.prefPilot.pilotId) selected = true;

                return <option value={pilot.characterID} selected={selected}>{pilot.characterName}</option>
            });
        }

        return (
            <div>
                <h6>Waitlist with:</h6>
                <select className="form-control" onChange={this.updateSelectedPilot.bind(this)} >
                    {pilotOptions}
                </select>
            </div>
        )
    }
}
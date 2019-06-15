import React, { Component } from 'react';
import Card from './Card';

export default class WaitlistUp extends Component {
    constructor(props) {
        super(props);

        this.state = {
            selectedPilot: null,
            selectedRoles: [],
            selectedFits: [],
            key: 0
        }
    }

    getPilots() {
        return this.props.pilots || null;
    }

    getPrefPilot() {
        return this.props.prefPilot || null;
    }

    getRoles() {
        return this.props.options ? this.props.options.roles : null;
    }

    getShips() {
        return this.props.options ? this.props.options.fittings : null;
    }

    updateSelectedPilot(pilot_id) {
        this.setState({ selectedPilot: pilot_id });
    }

    updateSelectedFits(isSelected, fit_id) {
        let fits = this.state.selectedFits;

        if (isSelected) {
            fits.push(fit_id);
        } else {

            for (let i = 0; i < fits.length; i++) {
                if (fits[i] === fit_id)
                    fits.splice(i, 1);
            }
        }

        this.setState({ selectedFits: fits });
    }

    updateSelectedRoles(isSelected, role_id) {
        let roles = this.state.selectedRoles;

        if (isSelected) {
            roles.push(role_id);
        } else {
            for (let i = 0; i < roles.length; i++) {
                if (roles[i] === role_id)
                    roles.splice(i, 1);
            }
        }

        this.setState({ selectedRoles: roles });
    }

    AddPilotToWaitlist() {   
        if (this.state.selectedFits == null || this.state.selectedFits.length == 0) {
            console.error("You must select at least one fit");
            return;
        }

        $.ajax({
            type: 'post',
            url: `${this.props.baseUri}`,
            data: {
                pilot_id: this.state.selectedPilot,
                role_ids: this.state.selectedRoles.join(),
                fit_ids: this.state.selectedFits.join()
            }
        }).done(() => {
            this.props.u();
            this.setState({ key: this.state.key + 1, selectedFits: [], selectedRoles: [] });
        }).fail((err) => {
            console.error(`React/Components/WaitlistUp {WaitlistUp@AddPilotToWaitlist} - Error joining the waitlist`, err.responseText);
        })  
    }

    render() {
        return (
            <div>
                <Card heading="Join the Waitlist">
                    <div className="row pb-4">
                        <div className="col-md-5 col-sm-12">
                            <WaitlistWith pilots={this.getPilots()} prefPilot={this.getPrefPilot()} selectedPilot={this.updateSelectedPilot.bind(this)} key={this.state.key} />
                        </div>

                        <div className="col-md-7 col-sm-12">
                            <SelectRoles roles={this.getRoles()} selectedRole={this.updateSelectedRoles.bind(this)} key={this.state.key} />
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-12">
                            <SelectShips fits={this.getShips()} selectedFit={this.updateSelectedFits.bind(this)} key={this.state.key} />
                            <div class="clearfix"></div>
                            <button className="btn btn-success" onClick={this.AddPilotToWaitlist.bind(this)}>Join the Waitlist <i className="fas fa-user-plus"></i></button>
                        </div>
                    </div>
                </Card>
            </div>
        )
    }
}

class SelectShips extends Component {    
    updateFits(e) {
        this.props.selectedFit(e.target.checked, Number(e.target.name));
    }

    render() {
        let fits;
        if (this.props.fits) {
            fits = this.props.fits.map((fit) => {
                return (
                    <div className="col-12">
                        <label className="custom-control custom-checkbox">
                            <input type="checkbox" className="custom-control-input" onChange={this.updateFits.bind(this)} name={fit.id} />
                            <span className="custom-control-indicator"></span>
                            <span className="custom-control-description"><img src={`https://image.eveonline.com/Render/${fit.typeId}_32.png`} /> {fit.description}</span>
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
    updateRoles(e) {
        this.props.selectedRole(e.target.checked, Number(e.target.name));
    }

    render() {
        let roles;
        if (this.props.roles) {
            roles = this.props.roles.map((role) => {
                return (
                    <label className="custom-control custom-checkbox">
                        <input type="checkbox" className="custom-control-input" onChange={this.updateRoles.bind(this)} name={role.id}/>
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

    render() {
        let pilotOptions;
        if (this.props.pilots != null) {
            pilotOptions = this.props.pilots.map((pilot) => {
                let selected = false;
                if (this.props.prefPilot && pilot.id == this.props.prefPilot.id) selected = true;

                return <option value={pilot.id} selected={selected}>{pilot.name}</option>
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